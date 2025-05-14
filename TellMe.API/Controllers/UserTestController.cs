using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models;
using TellMe.Service.Services.Interface;
using TellMe.API.Constants;
using TellMe.API.Helper;
using Sprache;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.UserTest.UserTestEndpoint)]
    [ApiController]
    public class UserTestController : ControllerBase
    {
        private readonly IUserTestService _userTestService;

        public UserTestController(IUserTestService userTestService)
        {
            _userTestService = userTestService;
        }

        /// <summary>
        /// Submits answers for a psychological test.
        /// </summary>
        /// <param name="request">The test answers submission.</param>
        /// <returns>Test results.</returns>
        [HttpPost("submit")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitTestAnswers([FromBody] SubmitUserTestRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                });

            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null)
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "User ID not found or invalid",
                    Data = null
                });

            var result = await _userTestService.SubmitTestAnswersAsync(userId.Value, request);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Test answers submitted successfully",
                Data = result
            });
        }

        /// <summary>
        /// Retrieves the user's test history.
        /// </summary>
        /// <param name="page">Page number (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>User test history.</returns>
        [HttpGet("history")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserTestHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null)
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "User ID not found or invalid",
                    Data = null
                });

            var history = await _userTestService.GetUserTestHistoryAsync(userId.Value, page, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved user test history",
                Data = history
            });
        }
    }
}
