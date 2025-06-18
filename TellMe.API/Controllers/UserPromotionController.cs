using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.UserPromotion.UserPromotionEndpoint)]
    [ApiController]
    public class UserPromotionController : ControllerBase
    {
        private readonly IUserPromotionService _userPromotionService;

        public UserPromotionController(IUserPromotionService userPromotionService)
        {
            _userPromotionService = userPromotionService ?? throw new ArgumentNullException(nameof(userPromotionService));
        }

        // GET: api/UserPromotion
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserPromotions()
        {
            try
            {
                var userPromotions = await _userPromotionService.GetAllUserPromotionsAsync();

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "All user promotions retrieved successfully",
                    Data = userPromotions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving user promotions: {ex.Message}",
                    Data = null
                });
            }
        }

        // GET: api/UserPromotion/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserPromotionById(int id)
        {
            try
            {
                var userPromotion = await _userPromotionService.GetUserPromotionByIdAsync(id);

                if (userPromotion == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"User promotion with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "User promotion retrieved successfully",
                    Data = userPromotion
                });
            }
            catch (ArgumentException ex)
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
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving user promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // GET: api/UserPromotion/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserPromotionsByUserId(Guid userId)
        {
            try
            {
                var userPromotions = await _userPromotionService.GetUserPromotionsByUserIdAsync(userId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "User promotions retrieved successfully",
                    Data = userPromotions
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
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
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving user promotions: {ex.Message}",
                    Data = null
                });
            }
        }

        // POST: api/UserPromotion
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserPromotion([FromBody] UserPromotionRequest request)
        {
            try
            {
                var userPromotion = await _userPromotionService.AddUserPromotionAsync(request);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "User promotion added successfully",
                    Data = userPromotion
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
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
            catch (InvalidOperationException ex)
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
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error adding user promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // PUT: api/UserPromotion/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserPromotion(int id, [FromBody] UserPromotionRequest request)
        {
            try
            {
                var result = await _userPromotionService.UpdateUserPromotionAsync(id, request);

                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"User promotion with ID {id} not found or update failed",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "User promotion updated successfully",
                    Data = null
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (ArgumentException ex)
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
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error updating user promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // DELETE: api/UserPromotion/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserPromotion(int id)
        {
            try
            {
                var result = await _userPromotionService.DeleteUserPromotionAsync(id);

                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"User promotion with ID {id} not found or delete failed",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "User promotion deleted successfully",
                    Data = null
                });
            }
            catch (ArgumentException ex)
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
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error deleting user promotion: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
