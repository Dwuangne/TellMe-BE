using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IAuthenticationService
    {  
        Task<AccountResponse> LoginAsync(LoginAccountRequest loginRequest);
        Task<bool> RegisterAsync(string baseUrl, RegisterAccountRequest registerRequest);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<AccountResponse> LoginGoogleAsync(LoginGoogleRequest loginGoogleRequest);
        Task<TokenResponse> RefreshTokenAsync(TokenRequest tokenRequest);
    }
}
