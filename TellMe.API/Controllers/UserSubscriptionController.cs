using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.UserSubscription.UserSubscriptionEndpoint)]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly IUserSubscriptionService _subscriptionService;

        public UserSubscriptionController(IUserSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 201)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateUserSubscriptionRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("UserId")?.Value!);
                var subscription = await _subscriptionService.CreateSubscriptionAsync(userId, request);

                return Created($"api/v1/usersubscriptions/{subscription.Id}", new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Subscription created successfully",
                    Data = subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetSubscription(Guid id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Subscription not found",
                    Data = null
                });
            }

            //var userId = Guid.Parse(User.FindFirst("UserId")?.Value!);
            //if (subscription.UserId != userId && !User.IsInRole("Admin"))
            //{
            //    return Forbid();
            //}

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Subscription retrieved successfully",
                Data = subscription
            });
        }

        [HttpGet("user")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> GetUserSubscriptions()
        {
            var userId = Guid.Parse(User.FindFirst("UserId")?.Value!);
            var subscriptions = await _subscriptionService.GetUserSubscriptionsAsync(userId);

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Subscriptions retrieved successfully",
                Data = subscriptions
            });
        }

        /// <summary>
        /// Get all subscriptions - Returns all subscriptions for admin, user's subscriptions for authenticated users
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        public async Task<IActionResult> GetAllSubscriptions([FromQuery] Guid? userId)
        {
            try
            {
                var subscriptions = await _subscriptionService.GetAllSubscriptionAsync(userId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved all subscriptions",
                    Data = subscriptions
                });
            }
            catch (FormatException)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid user ID format",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update a subscription's status
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateUserSubscriptionRequest request)
        {
            try
            {
                // Get current user ID
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ResponseObject
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Message = "User not authenticated",
                        Data = null
                    });
                }

                // Check if subscription exists and belongs to user or user is admin
                var existingSubscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                if (existingSubscription == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Subscription not found",
                        Data = null
                    });
                }

                // Verify ownership or admin role
                //if (existingSubscription.UserId != Guid.Parse(userId) && !User.IsInRole("Admin"))
                //{
                //    return StatusCode((int)HttpStatusCode.Forbidden, new ResponseObject
                //    {
                //        Status = HttpStatusCode.Forbidden,
                //        Message = "You can only update your own subscriptions",
                //        Data = null
                //    });
                //}

                var updatedSubscription = await _subscriptionService.UpdateSubscriptionAsync(id, request);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Subscription updated successfully",
                    Data = updatedSubscription
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
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}