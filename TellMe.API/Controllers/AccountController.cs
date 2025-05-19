using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Service.Exceptions;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Account.AccountEndpoint)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [Route(APIEndPointConstant.Account.ChangePassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.ChangePasswordAsync(request);
            if (!result)
                return BadRequest(new { message = MessageConstant.Account.ChangePasswordFailed });

            return Ok(new { message = MessageConstant.Account.ChangePasswordSuccessfully });
        }

        [HttpPost]
        [Route(APIEndPointConstant.Account.ForgotPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.ForgotPasswordAsync(request.Email);
            if (!result)
                return BadRequest(new { message = MessageConstant.Account.ForgotPasswordFailed });

            return Ok(new { message = MessageConstant.Account.ForgotPasswordSuccessfully });
        }

        [HttpPost]
        [Route(APIEndPointConstant.Account.ResetPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.ResetPasswordAsync(request.Email, request.ResetCode, request.NewPassword);
            if (!result)
                return BadRequest(new { message = MessageConstant.Account.ResetPasswordFailed });

            return Ok(new { message = MessageConstant.Account.ResetPasswordSuccessfully });
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _accountService.GetAllUserAsync();
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved all users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get user profile by ID (Admin or account owner)
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? string.Empty);
                if (currentUserId != userId)
                {
                    return Forbid();
                }

                var profile = await _accountService.GetProfileAsync(userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved user profile",
                    Data = profile
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get user profile by ID (Admin or account owner)
        /// </summary>
        [HttpGet("admin/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfileAdmin(Guid userId)
        {
            try
            {
                var profile = await _accountService.GetProfileAsync(userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved user profile",
                    Data = profile
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update user profile (Account owner)
        /// </summary>
        [HttpPut("{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? string.Empty);
                if (currentUserId != userId)
                {
                    return Forbid();
                }

                var updatedProfile = await _accountService.UpdateProfileAsync(request, userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully updated user profile",
                    Data = updatedProfile
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update user profile (Admin)
        /// </summary>
        [HttpPut("admin/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfileAdmin(Guid userId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var updatedProfile = await _accountService.UpdateProfileAsync(request, userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully updated user profile",
                    Data = updatedProfile
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Delete or restore user account (Admin only)
        /// </summary>
        [HttpPut("admin/{userId}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrRestoreUser(Guid userId, [FromQuery] bool status)
        {
            try
            {
                var result = await _accountService.DeleteOrRestoreUserAsync(userId, status);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = status ? "Successfully deleted user account" : "Successfully restored user account",
                    Data = result
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update user roles (Admin only)
        /// </summary>
        [HttpPut("admin/{userId}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoles(Guid userId, [FromBody] string[] roles)
        {
            try
            {
                var result = await _accountService.Decentralization(userId, roles);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully updated user roles",
                    Data = result
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
