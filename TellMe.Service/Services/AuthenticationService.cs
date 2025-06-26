using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Repository.Redis.Models;
using TellMe.Service.Constants;
using TellMe.Service.Exceptions;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;
        private readonly ITimeHelper _timeHelper;

        public AuthenticationService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService, IRedisService redisService, ITimeHelper timeHelper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _redisService = redisService;
            _timeHelper = timeHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                string confirmFailurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathTemplate.ConfirmEmailFailure);
                string confirmSuccessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathTemplate.ConfirmEmailSuccess);

                // Read the correct template based on the outcome
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                {
                    return false;
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return false;
                }
                // Decode the Base64 URL-encoded token
                byte[] decodedBytes = WebEncoders.Base64UrlDecode(token);
                string decodedToken = Encoding.UTF8.GetString(decodedBytes);

                // Confirm email with the decoded token
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    // Unlock the account
                    // Option 1: Disable lockout entirely
                    user.LockoutEnabled = false;

                    // Option 2: Reset lockout status (if you want to keep lockout enabled)
                    user.LockoutEnd = null; // Clear lockout end date
                    user.AccessFailedCount = 0; // Reset failed login attempts

                    // Update the user in the database
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        // Handle update failure (optional)
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        throw new Exception(errors);
                    }

                    var userForPatientProfile = await _userManager.FindByIdAsync(userId);
                    if (userForPatientProfile == null)
                    {
                        throw new NotFoundException(MessageConstant.Account.UpdateProfile.UserNotFound);
                    }

                    // Create a new PatientProfile if it doesn't exist
                    var existingProfile = await _unitOfWork.PatientProfileRepository.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userForPatientProfile.Id));
                    if (existingProfile == null)
                    {
                        var newProfile = new PatientProfile
                        {
                            UserId = Guid.Parse(userForPatientProfile.Id),
                            Name = userForPatientProfile.FullName,
                            Email = userForPatientProfile.Email,
                            IsActive = true,
                        };
                        await _unitOfWork.PatientProfileRepository.AddAsync(newProfile);
                        await _unitOfWork.CommitAsync();
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountResponse> LoginAsync(LoginAccountRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Authentication.Login.EmailNotFound);
                } else if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    throw new BadRequestException(MessageConstant.Authentication.Login.EmailNotVerified);
                }else if (user.LockoutEnabled)
                {
                    throw new BadRequestException(MessageConstant.Authentication.Login.AccountDisabled);
                }else if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    throw new BadRequestException(MessageConstant.Authentication.Login.InvalidCredentials);
                }

                var roles = await _userManager.GetRolesAsync(user) ?? new List<string>();
                var jwtId = Guid.NewGuid().ToString();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, jwtId) // Thêm Guid làm ID
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var accessToken = _tokenService.GenerateJwtToken(user, claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Tạo AccountToken để lưu vào Redis
                var accountToken = new AccountToken
                {
                    AccountId = user.Id,
                    JWTId = jwtId, 
                    RefreshToken = refreshToken,
                };

                // Lưu refresh token vào Redis
                await _redisService.AddAccountTokenAsync(accountToken);

                return new AccountResponse
                {
                    UserId = Guid.Parse(user.Id),
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToArray(),
                    Tokens = new TokenResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
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
                throw new Exception(ex.Message);
            }

        }

        public async Task<AccountResponse> LoginGoogleAsync(LoginGoogleRequest loginGoogleRequest)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(loginGoogleRequest.Token);

                // Nếu token hợp lệ, bạn có thể truy cập các thông tin từ payload
                // Ví dụ: email, name, và các claims khác
                var email = payload.Email;

                // Xử lý đăng nhập tại đây, ví dụ: kiểm tra người dùng trong cơ sở dữ liệu, tạo session, v.v.

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Nếu người dùng chưa tồn tại, tạo mới
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = payload.Name,
                        EmailConfirmed = true
                    };
                    var createResult = await _userManager.CreateAsync(newUser);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        throw new BadRequestException(errors);
                    }
                    createResult = await _userManager.AddToRoleAsync(newUser, "User");
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        throw new BadRequestException(errors);
                    }
                    user = newUser;
                }
                else
                {
                    if (user.PasswordHash != null)
                    {
                        throw new BadRequestException(MessageConstant.Authentication.Register.EmailExists);
                    }
                }
                var roles = await _userManager.GetRolesAsync(user) ?? new List<string>();
                var jwtId = Guid.NewGuid().ToString();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, jwtId) // Thêm Guid làm ID
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var accessToken = _tokenService.GenerateJwtToken(user, claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Tạo AccountToken để lưu vào Redis
                var accountToken = new AccountToken
                {
                    AccountId = user.Id,
                    JWTId = jwtId,
                    RefreshToken = refreshToken,
                };

                // Lưu refresh token vào Redis
                await _redisService.AddAccountTokenAsync(accountToken);

                return new AccountResponse
                {
                    UserId = Guid.Parse(user.Id),
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToArray(),
                    Tokens = new TokenResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                };
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(TokenRequest tokenRequest)
        {
            try
            {
                // Bước 1: Validate access token và trích xuất claims (không validate thời gian hết hạn)
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
                if (principal == null)
                {
                    throw new BadRequestException(MessageConstant.Authentication.RefreshToken.InvalidToken);
                }

                var userId = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var jwtId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jwtId))
                {
                    throw new BadRequestException(MessageConstant.Authentication.RefreshToken.InvalidToken);
                }

                // Bước 2: Kiểm tra token trong Redis
                var storedToken = await _redisService.GetAccountTokenAsync(userId);
                if (storedToken == null || storedToken.RefreshToken != tokenRequest.RefreshToken || storedToken.JWTId != jwtId)
                {
                    throw new BadRequestException(MessageConstant.Authentication.RefreshToken.ExpiredToken);
                }

                // Bước 3: Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.Authentication.RefreshToken.UserNotFound);
                }

                var roles = await _userManager.GetRolesAsync(user) ?? new List<string>();

                // Bước 4: Tạo token mới
                var newJwtId = Guid.NewGuid().ToString();
                var newClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, newJwtId)
                };
                newClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var newAccessToken = _tokenService.GenerateJwtToken(user, newClaims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Bước 5: Cập nhật token trong Redis
                storedToken.JWTId = newJwtId;
                storedToken.RefreshToken = newRefreshToken;
                await _redisService.UpdateAccountTokenAsync(storedToken);

                return new TokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
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

        public async Task<bool> RegisterAsync(string baseUrl, RegisterAccountRequest registerRequest)
        {
            try
            {
                // Check for existing user
                var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
                if (existingUser != null)
                {
                    throw new BadRequestException(MessageConstant.Authentication.Register.EmailExists);
                }

                // Create new user
                var identityUser = new ApplicationUser
                {
                    UserName = registerRequest.Email,
                    Email = registerRequest.Email,
                    FullName = registerRequest.FullName,
                    RegistrationDate = _timeHelper.NowVietnam(),
                    EmailConfirmed = false,
                    LockoutEnabled = false,
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerRequest.Password);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                    throw new BadRequestException(errors);
                }

                // Add roles if specified
                if (registerRequest.RoleId?.Any() == true)
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequest.RoleId);
                    if (!identityResult.Succeeded)
                    {
                        var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                        throw new BadRequestException(MessageConstant.Authentication.Register.RegistrationFailed + errors);
                    }
                }

                // Generate and send email confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = $"{baseUrl.TrimEnd('/')}?userId={identityUser.Id}&token={encodedToken}";

                var confirmEmailRequest = new ConfirmEmailRequest
                {
                    Email = identityUser.Email,
                    FullName = identityUser.FullName,
                    ConfirmationLink = confirmationLink
                };

                await _emailService.SendConfirmationEmailAsync(confirmEmailRequest);

                return true;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
