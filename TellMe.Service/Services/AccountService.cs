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
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(
            UserManager<ApplicationUser> userManager, 
            IEmailService emailService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _roleManager = roleManager;
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

        public async Task<bool> Decentralization(Guid userId, string[] roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.UpdateProfile.UserNotFound);
                }

                // Remove existing roles
                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                        throw new BadRequestException($"Error removing existing roles: {errors}");
                    }
                }

                // Validate and add new roles
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        throw new BadRequestException($"Role {role} does not exist");
                    }
                }

                // Add new roles
                var addResult = await _userManager.AddToRolesAsync(user, roles);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Error adding new roles: {errors}");
                }

                return true;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user roles: {ex.Message}");
            }
        }

        public async Task<bool> DeleteOrRestoreUserAsync(Guid userId, bool status)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.UpdateProfile.UserNotFound);
                }

                // true = delete, false = restore
                if (status)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                }
                else
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }

                return true;
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error managing user status: {ex.Message}");
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

        public async Task<IEnumerable<ProfileResponse>> GetAllUserAsync()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var profileResponses = new List<ProfileResponse>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    profileResponses.Add(new ProfileResponse
                    {
                        Id = Guid.Parse(user.Id),
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        Avatar = user.Avatar,
                        Address = user.Address,
                        RegistrationDate = user.RegistrationDate,
                        EmailConfirmed = user.EmailConfirmed,
                        LockoutEnabled = user.LockoutEnabled
                    });
                }

                return profileResponses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ProfileResponse> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.UpdateProfile.UserNotFound);
                }

                var roles = await _userManager.GetRolesAsync(user);

                return new ProfileResponse
                {
                    Id = Guid.Parse(user.Id),
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    RegistrationDate = user.RegistrationDate,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = !user.LockoutEnabled
                };
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving profile: {ex.Message}");
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

        public async Task<ProfileResponse> UpdateProfileAsync(UpdateProfileRequest request, Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Account.UpdateProfile.UserNotFound);
                }

                // Update user properties
                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.Avatar = request.Avatar;
                user.Address = request.Address;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }

                // Return updated profile
                return new ProfileResponse
                {
                    Id = userId,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    RegistrationDate = user.RegistrationDate,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = !user.LockoutEnabled
                };
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating profile: {ex.Message}");
            }
        }
    }
}
