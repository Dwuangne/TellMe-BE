using Microsoft.AspNetCore.Http;
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
                return BadRequest(new { message = TellMe.API.Constants.MessageConstant.AuthenticationMessage.ChangePasswordFailed });

            return Ok(new { message = TellMe.API.Constants.MessageConstant.AuthenticationMessage.ChangePasswordSuccessfully });
        }
    }
}
