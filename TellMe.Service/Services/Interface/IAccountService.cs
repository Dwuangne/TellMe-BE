using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IAccountService 
    {
        Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword);

        Task<ProfileResponse> UpdateProfileAsync(UpdateProfileRequest request, Guid userId);
        Task<ProfileResponse> GetProfileAsync(Guid userId);
        Task<IEnumerable<ProfileResponse>> GetAllUserAsync();
        Task<bool> DeleteOrRestoreUserAsync(Guid userId, bool status);
        Task<bool> Decentralization(Guid userId, string[] Roles);
    }
}
