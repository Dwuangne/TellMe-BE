using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models;
using TellMe.Service.Services.Interface;
using TellMe.API.Constants;

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
        /// Submit answers for a psychological test
        /// </summary>
        /// <param name="request">The test answers submission</param>
        /// <returns>Test results</returns>
        [HttpPost("submit")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> SubmitTestAnswers([FromBody] SubmitUserTestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid request data",
                    Data = ModelState
                });
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "User ID not found or invalid",
                    Data = null
                });
            }

            try
            {
                var result = await _userTestService.SubmitTestAnswersAsync(userId, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Test answers submitted successfully",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("history")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        public async Task<IActionResult> GetUserTestHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "User ID not found or invalid",
                    Data = null
                });
            }

            var history = await _userTestService.GetUserTestHistoryAsync(userId, page, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved user test history",
                Data = history
            });
        }
    }
}
