using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoolJobAPI.Models;
using CoolJobAPI.Models.DTO.Responses;
using System.Security.Claims;
using CoolJobAPI.Models.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CoolJobAPI.Interfaces;
using CoolJobAPI.Services;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;


        // Make a repository and move the full token logic to there
        public UsersController(IUserRepository userRepository, JwtTokenHandler jwtTokenHandler, IRefreshTokenRepository refreshTokenRepository) 
        {
            _userRepository = userRepository;
            _jwtTokenHandler = jwtTokenHandler;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] UserRegistrationRequestDto user)
        {
            if (!ModelState.IsValid)
            {
                // Get the validation related error massages
                IEnumerable<string> errorMassages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ErrorResponse(errorMassages));
            }

            if (user.Password != user.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("The password and the confirmation password is not equal"));
            }

            if (await _userRepository.GetByEmail(user.Email) != null)
            {
                return Conflict(new ErrorResponse("Email is already exist"));
            }

            if (await _userRepository.GetByUserName(user.UserName) != null)
            {
                return Conflict(new ErrorResponse("User name is already exist"));
            }

            var newUser = await _userRepository.CreateUser(user);

            if (newUser != null)
            {
                var token = _jwtTokenHandler.GenerateToken(newUser);
                var refreshToken = _jwtTokenHandler.GenerateRefreshToken();

                return Ok(new SuccessfulAuthResponse() { Token = token, RefreshToken = refreshToken });
            }

            return BadRequest(new ErrorResponse("Create a new user is not possible"));
        }

        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMassages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ErrorResponse(errorMassages));
            }

            var existingUser = await _userRepository.GetByEmail(user.Email);

            if (existingUser == null)
            {
                return Unauthorized();
            }

            var isThePasswordValid = await _userRepository.CheckThePassword(existingUser, user.Password);

            if (!isThePasswordValid)
            {
                return Unauthorized();
            }

            var oldRefreshToken = await _refreshTokenRepository.GetTokenByUserId(existingUser.Id);

            if (oldRefreshToken != null)
            {
                var successDelete = await _refreshTokenRepository.RemoveTokenById(oldRefreshToken.Id);

                if (!successDelete)
                {
                    return BadRequest(new ErrorResponse("Deleting the old RefreshToken is not possible"));
                }
            }

            var token = _jwtTokenHandler.GenerateToken(existingUser);
            var refreshToken = _jwtTokenHandler.GenerateRefreshToken();

            RefreshToken newRefreshToken = new RefreshToken()
            {
                Token = refreshToken,
                UserId = existingUser.Id,
                User = existingUser,
            };

            var success = await _refreshTokenRepository.SaveToken(newRefreshToken);

            if (!success)
            {
                return BadRequest(new ErrorResponse("Save a new RefreshToken is not possible"));
            }

            return Ok(new SuccessfulAuthResponse() { Token = token, RefreshToken = refreshToken });
        }
        
        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMassages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ErrorResponse(errorMassages));
            }

            var isValid = _jwtTokenHandler.ValidateRefreshToken(tokenRequest.RefreshToken);

            if (!isValid)
            {
                return BadRequest(new ErrorResponse("Token refreshing is not possible"));
            }

            var refreshToken = await _refreshTokenRepository.GetByToken(tokenRequest.RefreshToken);

            if (refreshToken == null)
            {
                return NotFound("RefreshToken is not exists");
            }

            var user = await _userRepository.GetById(refreshToken.UserId);

            if (user == null)
            {
                return NotFound("User is not exists");
            }

            var successDelete = await _refreshTokenRepository.RemoveTokenById(refreshToken.Id);

            if (!successDelete)
            {
                return BadRequest(new ErrorResponse("Deleting the old RefreshToken is not possible"));
            }

            var token = _jwtTokenHandler.GenerateToken(user);

            RefreshToken newRefreshToken = new RefreshToken()
            {
                Token = _jwtTokenHandler.GenerateRefreshToken(),
                UserId = user.Id,
                User = user
            };

            var successSave = await _refreshTokenRepository.SaveToken(newRefreshToken);

            if (!successSave)
            {
                return BadRequest(new ErrorResponse("Save a new RefreshToken is not possible"));
            }

            return Ok(new SuccessfulAuthResponse() { Token = token, RefreshToken = newRefreshToken.Token });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Get the user ID from the Token
            var userId = HttpContext.User.FindFirstValue("Id");

            if (userId == null || userId.Length == 0)
            {
                return Unauthorized();
            }

            var successDelete = await _refreshTokenRepository.RemoveTokenByUserId(userId);

            if (!successDelete)
            {
                return BadRequest(new ErrorResponse("Failed logout - Can't delete the refresh token"));
            }

            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteRegistration")]
        public async Task<IActionResult> DeleteRegistration([FromBody] UserLoginRequestDto userRequest)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMassages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new ErrorResponse(errorMassages));
            }

            var userId = HttpContext.User.FindFirstValue("Id");
            var userByEmail = await _userRepository.GetByEmail(userRequest.Email);

            if (userId == null || userId.Length == 0 || userByEmail == null)
            {
                return BadRequest(new ErrorResponse("User is not exists"));
            }

            var user = await _userRepository.GetById(userId);

            var isThePasswordValid = await _userRepository.CheckThePassword(user, userRequest.Password);

            if (!isThePasswordValid)
            {
                return Unauthorized();
            }

            // If we want to delete a row from a parent table, and we also want to delete the related row from the child table
            // then we have to set up this: onDelete: ReferentialAction.Cascade
            // in the migration file at the child tables where the foreign key is set
            var successDelete = await _userRepository.DeleteUser(user);

            if (!successDelete)
            {
                return BadRequest(new ErrorResponse("Can't delete the user"));
            }

            return Ok();
        }
    }
}
