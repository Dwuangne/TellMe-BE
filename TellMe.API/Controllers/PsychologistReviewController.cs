using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using TellMe.API.Constants;
using TellMe.Service.Models;
using TellMe.Service.Services.Interface;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.PsychologistReview.PsychologistReviewEndpoint)]
    [ApiController]
    public class PsychologistReviewController : Controller
    {
        private readonly IPsychologistReviewService _reviewService;

        public PsychologistReviewController(IPsychologistReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get all reviews for a specific psychologist with pagination
        /// </summary>
        /// <param name="psychologistId">The ID of the psychologist</param>
        /// <param name="pageIndex">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated list of reviews</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetPsychologistReviews(
            [FromQuery] Guid? psychologistId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginationParams = new PaginationParams
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                var (reviews, totalPages, totalRecords) = await _reviewService.GetReviewsByPsychologistIdAsync(
                    psychologistId, 
                    paginationParams.PageIndex, 
                    paginationParams.PageSize);

                var paginatedResponse = new PaginatedResponse<PsychologistReviewResponse>
                {
                    Items = reviews,
                    PageIndex = paginationParams.PageIndex,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords
                };

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved reviews",
                    Data = paginatedResponse
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get all reviews for a specific psychologist with pagination
        /// </summary>
        /// <param name="psychologistId">The ID of the psychologist</param>
        /// <param name="pageIndex">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated list of reviews</returns>
        [HttpGet("active/{psychologistId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetPsychologistReviewsActive(
            Guid psychologistId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginationParams = new PaginationParams
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                var (reviews, totalPages, totalRecords) = await _reviewService.GetReviewsActiveByPsychologistIdAsync(
                    psychologistId,
                    paginationParams.PageIndex,
                    paginationParams.PageSize);

                var paginatedResponse = new PaginatedResponse<PsychologistReviewResponse>
                {
                    Items = reviews,
                    PageIndex = paginationParams.PageIndex,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords
                };

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved reviews",
                    Data = paginatedResponse
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get a specific review by ID
        /// </summary>
        /// <param name="reviewId">The ID of the review</param>
        /// <returns>Review details</returns>
        [HttpGet("{reviewId}/psychologist")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Review not found",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved review",
                Data = review
            });
        }

        /// <summary>
        /// Add a new review for a psychologist
        /// </summary>
        /// <param name="psychologistId">The ID of the psychologist being reviewed</param>
        /// <param name="rating">Rating value (0-5)</param>
        /// <param name="comment">Review comment</param>
        /// <returns>Created review</returns>
        [HttpPost("psychologist/{psychologistId}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ResponseObject), 201)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> AddReview(Guid psychologistId, [FromBody] AddReviewRequest request)
        {
            try
            {
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

                var review = await _reviewService.AddReviewAsync(userId, psychologistId, request.Rating, request.Comment);

                return Created($"api/v1/psychologist-reviews/{review.Id}", new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Review added successfully",
                    Data = review
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
            catch (InvalidOperationException ex)
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
        /// Update an existing review
        /// </summary>
        /// <param name="reviewId">The ID of the review to update</param>
        /// <param name="request">Update review request</param>
        /// <returns>Updated review</returns>
        [HttpPut("{reviewId}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewRequest request)
        {
            try
            {
                // Get current user ID
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ResponseObject
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Message = "User ID not found or invalid",
                        Data = null
                    });
                }

                // Get the review to check ownership
                var existingReview = await _reviewService.GetReviewByIdAsync(reviewId);
                if (existingReview == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Review not found",
                        Data = null
                    });
                }

                // Verify ownership
                if (existingReview.UserId != userId)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new ResponseObject
                    {
                        Status = HttpStatusCode.Forbidden,
                        Message = "You can only update your own reviews",
                        Data = null
                    });
                }

                var review = await _reviewService.UpdateReviewAsync(reviewId, request.Rating, request.Comment);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Review updated successfully",
                    Data = review
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Delete a review
        /// </summary>
        /// <param name="reviewId">The ID of the review to delete</param>
        /// <returns>Success status</returns>
        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                // Get current user ID
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ResponseObject
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Message = "User ID not found or invalid",
                        Data = null
                    });
                }

                var result = await _reviewService.DeleteReviewAsync(reviewId);
                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Review not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Review deleted successfully",
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
        /// Restore a review
        /// </summary>
        /// <param name="reviewId">The ID of the review to restore</param>
        /// <returns>Restored review</returns>
        [HttpPut("{reviewId}/restore")]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> RestoreReview(int reviewId)
        {
            try
            {
                // Get current user ID
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new ResponseObject
                    {
                        Status = HttpStatusCode.Unauthorized,
                        Message = "User ID not found or invalid",
                        Data = null
                    });
                }

                // Get the review to check ownership
                var existingReview = await _reviewService.GetReviewByIdAsync(reviewId);
                if (existingReview == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Review not found",
                        Data = null
                    });
                }

                // Can only restore inactive reviews
                if (existingReview.IsActive)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Review is already active",
                        Data = null
                    });
                }

                var result = await _reviewService.RestoreReviewAsync(reviewId);
                if (!result)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Failed to restore review",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Review restored successfully",
                    Data = await _reviewService.GetReviewByIdAsync(reviewId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = $"An error occurred while restoring the review: {ex.Message}",
                    Data = null
                });
            }
        }
    }

    public class AddReviewRequest
    {
        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public byte Rating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateReviewRequest
    {
        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public byte Rating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; } = string.Empty;
    }
}
