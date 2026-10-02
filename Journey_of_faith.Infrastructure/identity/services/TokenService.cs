using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Journey_of_faith.Infrastructure.identity.services
{
    public class TokenService(IConfiguration config, ILogger<TokenService> logger)
    {

        public string GenerateToken(ApplicationUser user, List<string> roles, List<string> claims)
        {


            var claim = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if(roles.Any())
            {
                foreach(var role in roles)
                {
                    claim.Add(new Claim("role", role.ToString()));
                }
            }

            if(claims.Any())
            {
                foreach(var claimIn in claims)
                {
                    claim.Add(new Claim("Permission", claimIn));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Token:Key") ?? string.Empty));

            var signa = new SigningCredentials(key, algorithm: SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config.GetValue<string>("Token:Issuer"),
                audience: config.GetValue<string>("Token:Audience"),
                claims: claim,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signa
            );


            var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            logger.LogInformation("Generated access token for user {UserId}", user.Id);
            return serializedToken;
        }


        public RefreshToken CreateRefreshToken(Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                UserId = userId
            };

            logger.LogInformation("Generated refresh token for user {UserId}", userId);
            return refreshToken;
        }
    }
}
