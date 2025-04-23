using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TellMe.Repository.Enities;
using TellMe.Service.Constants;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(ApplicationUser user, List<Claim> claims)
        {
            try
            {
                // Validate configuration values
                var jwtKey = _configuration["JwtAuth:Key"];
                var jwtIssuer = _configuration["JwtAuth:Issuer"];
                var jwtAudience = _configuration["JwtAuth:Audience"];

                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
                {
                    throw new InvalidOperationException(MessageConstant.JWTMessage.InvalidOperationJWT);
                }

                // Create security key and signing credentials
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Generate JWT token
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate JWT token: {ex.Message}", ex);
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var jwtKey = _configuration["JwtAuth:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException(MessageConstant.JWTMessage.InvalidOperationJWT);
                }
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // Cho phép lấy token đã hết hạn
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                // Kiểm tra token có phải JWT không
                if (securityToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }

                throw new SecurityTokenException("Token không hợp lệ.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
