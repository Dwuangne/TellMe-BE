using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Exceptions;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                // Tìm người dùng theo email
                var user = await _userManager.FindByEmailAsync(changePasswordRequest.Email);
                if (user == null)
                {
                    throw new NotFoundException("Email không tồn tại trong hệ thống.");
                }

                // Xác nhận người dùng đã xác thực email và không bị khóa
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    throw new BadRequestException("Email chưa được xác thực.");
                }
                if (user.LockoutEnabled)
                {
                    throw new BadRequestException("Tài khoản đang bị khóa.");
                }

                // Đổi mật khẩu
                var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Không thể đổi mật khẩu: {errors}");
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
