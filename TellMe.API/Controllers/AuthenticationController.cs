using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TellMe.API.Constants;
using TellMe.Service.Constants;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Authentication.AuthenticationEndpoint)]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route(APIEndPointConstant.Authentication.Login)]
        public async Task<IActionResult> Login([FromBody] LoginAccountRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.LoginAsync(loginRequest);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route(APIEndPointConstant.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterAccountRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var baseUrl = $"{Request.Scheme}://{Request.Host}{APIEndPointConstant.Authentication.AuthenticationEndpoint}/{APIEndPointConstant.Authentication.ConfirmEmail}";
            var result = await _authenticationService.RegisterAsync(baseUrl, registerRequest);
            return Ok(new
            {
                success = result,
                message = TellMe.API.Constants.MessageConstant.AuthenticationMessage.RegisterAccountSuccess,
            });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route(APIEndPointConstant.Authentication.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            string confirmFailurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TellMe.Service.Constants.PathConstant.PathTemplate.ConfirmEmailFailure);
            string confirmSuccessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TellMe.Service.Constants.PathConstant.PathTemplate.ConfirmEmailSuccess);

            var result = await _authenticationService.ConfirmEmailAsync(userId, token);
            string templatePath = result ? confirmSuccessPath : confirmFailurePath;
            return Content(await System.IO.File.ReadAllTextAsync(templatePath), "text/html");
        }

        [HttpPost]
        [Route(APIEndPointConstant.Authentication.LoginGoogle)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginGoogleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.LoginGoogleAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route(APIEndPointConstant.Authentication.RefreshToken)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.RefreshTokenAsync(request);
            return Ok(result);
        } 
    }
}
