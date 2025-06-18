using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Repository.Enities;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Promotion.PromotionEndpoint)]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        }

        // GET: api/Promotion
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPromotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllAsync();
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Promotions retrieved successfully",
                    Data = promotions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = $"Error retrieving promotions: {ex.Message}",
                    Data = null
                });
            }
        }

        // GET: api/Promotion/active
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivePromotions()
        {
            try
            {
                var activePromotions = await _promotionService.GetActivePromotionsAsync();
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Active promotions retrieved successfully",
                    Data = activePromotions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving active promotions: {ex.Message}",
                    Data = null
                });
            }
        }

        // GET: api/Promotion/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPromotion(int id)
        {
            try
            {
                var promotion = await _promotionService.GetByIdAsync(id);

                if (promotion == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"Promotion with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Promotion retrieved successfully",
                    Data = promotion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // POST: api/Promotion
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionRequest promotionRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid promotion data",
                    Data = null
                });
            }

            try
            {
                var createdPromotion = await _promotionService.CreateAsync(promotionRequest);

                var response = new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Promotion created successfully",
                    Data = createdPromotion
                };

                return CreatedAtAction(nameof(GetPromotion), new { id = createdPromotion.Id }, response);
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
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error creating promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // PUT: api/Promotion/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] PromotionRequest promotionRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid promotion data",
                    Data = null
                });
            }

            try
            {
                var updatedPromotion = await _promotionService.UpdateAsync(id, promotionRequest);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Promotion updated successfully",
                    Data = updatedPromotion
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"Promotion with ID {id} not found",
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
                    Message = $"Error updating promotion: {ex.Message}",
                    Data = null
                });
            }
        }

        // DELETE: api/Promotion/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                var result = await _promotionService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"Promotion with ID {id} not found",
                        Data = false
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Promotion deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = $"Error deleting promotion: {ex.Message}",
                    Data = false
                });
            }
        }

    }
}
