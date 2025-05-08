using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Services.Interface
{
    public interface IAccountService 
    {
        Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword);
    }
}
