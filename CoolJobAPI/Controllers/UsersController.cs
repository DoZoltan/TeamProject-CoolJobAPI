using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using CoolJobAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using CoolJobAPI.Models.DTO.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using CoolJobAPI.Models.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using CoolJobAPI.Repositories;
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

            var oldRefreshToken = await _refreshTokenRepository.GetPreviousToken(existingUser.Id);

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
                User = existingUser
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

            var successDelete = await _refreshTokenRepository.RemoveTokenById(refreshToken.Id);

            if (!successDelete)
            {
                return BadRequest(new ErrorResponse("Deleting the old RefreshToken is not possible"));
            }

            var user = await _userRepository.GetById(refreshToken.UserId);

            if (user == null)
            {
                return NotFound("User is not exists");
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
    }
}
