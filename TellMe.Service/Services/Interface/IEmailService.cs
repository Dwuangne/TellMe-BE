using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Services.Interface
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(ConfirmEmailRequest request);
        Task SendForgotPasswordEmailAsync(ForgotPasswordEmailRequest request);
    }
}
