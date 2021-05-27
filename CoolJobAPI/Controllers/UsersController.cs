using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using CoolJobAPI.Models;
using CoolJobAPI.Configuration;
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
using CoolJobAPI.Domain;
using Microsoft.EntityFrameworkCore;
using CoolJobAPI.Repositories;
using CoolJobAPI.Interfaces;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;


        // Make a repository and move the full token logic to there
        public UsersController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        [HttpPost("Registration")]
        public async Task<ActionResult<RegistrationResponse>> Registration([FromBody] UserRegistrationRequestDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Required registration fields are not filled"
                    }
                });

            }

            if (user.Password != user.ConfirmPassword)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "The password and the confirmation password is not equal"
                    }
                });
            }

            if (await _userRepository.GetByEmail(user.Email) != null)
            {
                return Conflict(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email is already exist"
                    }
                });
            }

            if (await _userRepository.GetByUserName(user.Email) != null)
            {
                return Conflict(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "User name is already exist"
                    }
                });
            }

            var newUser = await _userRepository.CreateUser(user);

            if (newUser != null)
            {
                //var jwtToken = await GenerateJwtToken(newUser);

                return Ok();
            }

            return BadRequest(new RegistrationResponse()
            {
                Result = false,
                Errors = new List<string>()
                    {
                        "Create a new user is not possible"
                    }
            });
        }

        /*
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto user)
        {
            if (ModelState.IsValid)
            {
                // check if the user with the same email exist
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    // We dont want to give to much information on why the request has failed for security reasons
                    return BadRequest(new RegistrationResponse()
                    {
                        Result = false,
                        Errors = new List<string>(){
                                        "Invalid authentication request"
                                    }
                    });
                }

                // Now we need to check if the user has inputed the right password
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (isCorrect)
                {
                    var jwtToken = await GenerateJwtToken(existingUser);

                    return Ok(jwtToken);
                }
                else
                {
                    // We dont want to give to much information on why the request has failed for security reasons
                    return BadRequest(new RegistrationResponse()
                    {
                        Result = false,
                        Errors = new List<string>(){
                                         "Invalid authentication request"
                                    }
                    });
                }
            }

            return BadRequest(new RegistrationResponse()
            {
                Result = false,
                Errors = new List<string>(){
                                        "Invalid payload"
                                    }
            });
        }

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

        private async Task<AutResult> GenerateNewToken(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //  it throws error if the token is expired... so this logic is not works
                // var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // So I (Zoli) using ReadJwtToken method instead of ValidateToken
                var validatedToken = jwtTokenHandler.ReadJwtToken(tokenRequest.Token);
                var utcExpiryDate = long.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Now we need to check if the token has a valid security algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Check if the token is not expired yet
                var expDatee = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDatee > DateTime.UtcNow)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "We cannot refresh this since the token has not expired" },
                        Result = false
                    };
                }

                // Check the token we got if its saved in the db
                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "refresh token doesnt exist" },
                        Result = false
                    };
                }

                // Check the date of the saved token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "token has expired, user needs to relogin" },
                        Result = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "token has been used" },
                        Result = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "token has been revoked" },
                        Result = false
                    };
                }

                // we are getting here the jwt token id
                var jti = validatedToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new AutResult()
                    {
                        Errors = new List<string>() { "the token doenst matched the saved token" },
                        Result = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                return new AutResult()
                {
                    Errors = new List<string>() { ex.ToString() },
                    Result = false
                };
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        private async Task<AutResult> GenerateJwtToken(IdentityUser user)
        {
            // define the jwt token which will be responsible of creating our tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // our secret from the appsettings
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                // the life span of the token
                Expires = DateTime.UtcNow.AddSeconds(30),
                // here we are adding the encryption alogorithim information which will be used to decrypt our token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AutResult()
            {
                Token = jwtToken,
                Result = true,
                RefreshToken = refreshToken.Token
            };
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
