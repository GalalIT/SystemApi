using Application.System.Interface.IUserOperation;
using Domin.System.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.UserServices
{
    public class JwtTokenService : ITokenGenerationOperation
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;
        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<string> GenerateTokenAsync(ApplicationUser user, IList<string> roles)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWTSetting:SecretKey"]);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWTSetting:ValidAudience"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWTSetting:ValidIssuer"])
            };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token generation failed");
                throw;
            }
        }
    }
}
