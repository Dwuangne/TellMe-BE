using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TellMe.API.Constants;
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
    }
}
