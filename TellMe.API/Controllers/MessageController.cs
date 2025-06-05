using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TellMe.API.Constants;
using TellMe.API.Helper;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Message.MessageEndpoint)]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Get messages by conversation ID with pagination
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="pageIndex">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 20)</param>
        /// <returns>Paginated list of messages</returns>
        [HttpGet("conversation/{conversationId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMessagesByConversationId(
            Guid conversationId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var messages = await _messageService.GetMessagesByConversationIdAsync(conversationId, pageIndex, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved messages",
                Data = messages
            });
        }

        /// <summary>
        /// Get message by ID
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <returns>Message details</returns>
        [HttpGet("{messageId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMessageById(Guid messageId)
        {
            var message = await _messageService.GetMessageByIdAsync(messageId);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved message",
                Data = message
            });
        }

        /// <summary>
        /// Send a new message
        /// </summary>
        /// <param name="request">Message creation request</param>
        /// <returns>Created message</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
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

            // Verify the user is sending the message themselves
            var currentUserId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (currentUserId == null)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid token or user ID not found",
                    Data = null
                });
            }

            if (currentUserId != request.UserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var message = await _messageService.AddMessageAsync(request);
            return StatusCode(StatusCodes.Status201Created, new ResponseObject
            {
                Status = HttpStatusCode.Created,
                Message = "Message sent successfully",
                Data = message
            });
        }

        /// <summary>
        /// Delete a message
        /// </summary>
        /// <param name="messageId">Message ID to delete</param>
        /// <returns>Success status</returns>
        [HttpDelete("{messageId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var currentUserId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (currentUserId == null)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid token or user ID not found",
                    Data = null
                });
            }

            var result = await _messageService.DeleteMessageAsync(messageId, currentUserId.Value);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Message deleted successfully",
                Data = result
            });
        }
    }
} 