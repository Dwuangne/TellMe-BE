using System.IdentityModel.Tokens.Jwt;

namespace TellMe.API.Helper
{
    public static class JwtHelper
    {
        public static Guid? GetUserIdFromToken(HttpRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Lấy Authorization header
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                errorMessage = "Missing or invalid Authorization header.";
                return null;
            }

            var token = authHeader.Split(" ").Last();

            try
            {
                // Giải mã token
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    errorMessage = "Invalid token.";
                    return null;
                }

                // Lấy UserId từ claim
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    errorMessage = "Invalid UserId in token.";
                    return null;
                }

                return userId;
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred: {ex.Message}";
                return null;
            }
        }
    }
}
