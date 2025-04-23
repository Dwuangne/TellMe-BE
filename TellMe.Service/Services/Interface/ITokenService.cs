using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;

namespace TellMe.Service.Services.Interface
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user, List<Claim> claims);
        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
