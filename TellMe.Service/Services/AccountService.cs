using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Constants;
using TellMe.Service.Exceptions;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public AccountService(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                // Tìm người dùng theo email
                var user = await _userManager.FindByEmailAsync(changePasswordRequest.Email);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.ChangePassword.EmailNotFound);
                }

                // Xác nhận người dùng đã xác thực email và không bị khóa
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    throw new BadRequestException(MessageConstant.Account.ChangePassword.EmailNotVerified);
                }
                if (user.LockoutEnabled)
                {
                    throw new BadRequestException(MessageConstant.Account.ChangePassword.AccountDisabled);
                }

                // Đổi mật khẩu
                var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }

                return true;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                // Tìm người dùng theo email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.ForgotPassword.EmailNotFound);
                }

                // Xác nhận người dùng đã xác thực email và không bị khóa
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    throw new BadRequestException(MessageConstant.Account.ForgotPassword.EmailNotVerified);
                }
                if (user.LockoutEnabled)
                {
                    throw new BadRequestException(MessageConstant.Account.ForgotPassword.AccountDisabled);
                }

                // Tạo token đặt lại mật khẩu
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = $"https://yourdomain.com/reset-password?email={user.Email}&token={token}";
                await _emailService.SendForgotPasswordEmailAsync(new ForgotPasswordEmailRequest
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    ResetPasswordLink = resetLink
                });

                return true;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.ResetPassword.EmailNotFound);
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    throw new BadRequestException(MessageConstant.Account.ResetPassword.EmailNotVerified);
                }
                if (user.LockoutEnabled)
                {
                    throw new BadRequestException(MessageConstant.Account.ResetPassword.AccountDisabled);
                }

                var result = await _userManager.ResetPasswordAsync(user, resetCode, newPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }

                return true;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
