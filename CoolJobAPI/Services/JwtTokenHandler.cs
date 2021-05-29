using CoolJobAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoolJobAPI.Services
{
    public class JwtTokenHandler
    {
        private readonly JwtConfig _jwtConfig;

        public JwtTokenHandler(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }

        public string GenerateToken(User user)
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

            return jwtTokenHandler.WriteToken(token);

            /*
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
            */
        }




        /*


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
            */
    }
}
