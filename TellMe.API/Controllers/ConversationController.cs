using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.API.Helper;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Conversation.ConversationEndpoint)]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        /// <summary>
        /// Get all conversations with pagination
        /// </summary>
        /// <param name="pageIndex">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 12)</param>
        /// <returns>Paginated list of conversations</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllConversations([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 12)
        {
            var conversations = await _conversationService.GetAllConversationAsync(pageIndex, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved all conversations",
                Data = conversations
            });
        }

        /// <summary>
        /// Get conversation by ID with optional message inclusion
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="includeMessages">Include messages in response (default: false)</param>
        /// <param name="messagePageIndex">Message page number (default: 1)</param>
        /// <param name="messagePageSize">Number of messages per page (default: 20)</param>
        /// <returns>Conversation details</returns>
        [HttpGet("{conversationId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetConversationById(
            Guid conversationId, 
            [FromQuery] bool includeMessages = false,
            [FromQuery] int messagePageIndex = 1,
            [FromQuery] int messagePageSize = 20)
        {
            var conversation = await _conversationService.GetConversationByIdAsync(
                conversationId, includeMessages, messagePageIndex, messagePageSize);
            
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved conversation",
                Data = conversation
            });
        }

        /// <summary>
        /// Get conversations by user ID with pagination
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageIndex">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 12)</param>
        /// <returns>Paginated list of user's conversations</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetConversationsByUserId(
            Guid userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Verify user can only access their own conversations (unless admin)
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

            if (currentUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var conversations = await _conversationService.GetConversationByUserIdAsync(userId, pageIndex, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved user conversations",
                Data = conversations
            });
        }

        /// <summary>
        /// Get current user's conversations with pagination
        /// </summary>
        /// <param name="pageIndex">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 12)</param>
        /// <returns>Paginated list of current user's conversations</returns>
        [HttpGet("my-conversations")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyConversations(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
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

            var conversations = await _conversationService.GetConversationByUserIdAsync(currentUserId.Value, pageIndex, pageSize);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved your conversations",
                Data = conversations
            });
        }

        /// <summary>
        /// Create a new conversation
        /// </summary>
        /// <param name="request">Conversation creation request</param>
        /// <returns>Created conversation</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateConversation([FromBody] ConversationRequest request)
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

            var conversation = await _conversationService.AddConversationAsync(request);
            return StatusCode(StatusCodes.Status201Created, new ResponseObject
            {
                Status = HttpStatusCode.Created,
                Message = "Conversation created successfully",
                Data = conversation
            });
        }

        /// <summary>
        /// Delete a conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID to delete</param>
        /// <returns>Success status</returns>
        [HttpDelete("{conversationId}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteConversation(Guid conversationId)
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

            var result = await _conversationService.DeleteConversationAsync(conversationId, currentUserId.Value);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Conversation deleted successfully",
                Data = result
            });
        }
    }
} 