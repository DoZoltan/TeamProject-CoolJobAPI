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


        // Make a repository and move the full token logic to there
        public UsersController(IUserRepository userRepository, JwtTokenHandler jwtTokenHandler) 
        {
            _userRepository = userRepository;
            _jwtTokenHandler = jwtTokenHandler;
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
                // Get the validation related error massages
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

            var token = _jwtTokenHandler.GenerateToken(existingUser);
            var refreshToken = _jwtTokenHandler.GenerateRefreshToken();

            return Ok(new SuccessfulAuthResponse() { Token = token, RefreshToken = refreshToken });
        }

        /*
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var res = await GenerateNewToken(tokenRequest);

                if (res == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                    "Invalid tokens"
                },
                        Result = false
                    });
                }

                return Ok(res);
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() {
                "Invalid payload"
            },
                Result = false
            });
        }

        

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }


        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        */
    }
}
