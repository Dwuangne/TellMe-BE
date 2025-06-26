using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PsychologicalAssessmentController : ControllerBase
    {
        private readonly IPsychologicalAssessmentService _psychologicalAssessmentService;

        public PsychologicalAssessmentController(IPsychologicalAssessmentService psychologicalAssessmentService)
        {
            _psychologicalAssessmentService = psychologicalAssessmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssessments([FromQuery] Guid? userId = null, [FromQuery] Guid? expertId = null)
        {
            try
            {
                var assessments = await _psychologicalAssessmentService.GetAllPsychologicalAssessmentsAsync(userId, expertId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Psychological assessments retrieved successfully",
                    Data = assessments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving psychological assessments",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveAssessments([FromQuery] Guid? userId = null, [FromQuery] Guid? expertId = null)
        {
            try
            {
                var assessments = await _psychologicalAssessmentService.GetAllActivePsychologicalAssessmentsAsync(userId, expertId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Active psychological assessments retrieved successfully",
                    Data = assessments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving active psychological assessments",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssessment(int id)
        {
            try
            {
                var assessment = await _psychologicalAssessmentService.GetPsychologicalAssessmentAsync(id);

                if (assessment == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"Psychological assessment with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Psychological assessment retrieved successfully",
                    Data = assessment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving psychological assessment",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssessment([FromBody] CreatePsychologicalAssessmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Invalid model state",
                        Data = ModelState
                    });
                }

                var assessment = await _psychologicalAssessmentService.CreatePsychologicalAssessmentAsync(request);

                var response = new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Psychological assessment created successfully",
                    Data = assessment
                };

                return CreatedAtAction(nameof(GetAssessment), new { id = assessment.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error creating psychological assessment",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAssessment(int id, [FromBody] UpdatePsychologicalAssessmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Invalid model state",
                        Data = ModelState
                    });
                }

                var assessment = await _psychologicalAssessmentService.UpdatePsychologicalAssessmentAsync(id, request);

                if (assessment == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"Psychological assessment with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Psychological assessment updated successfully",
                    Data = assessment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error updating psychological assessment",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id:int}/status")]
        public IActionResult ManageAssessmentStatus(int id, [FromQuery] bool isActive = false)
        {
            try
            {
                var result = _psychologicalAssessmentService.ManageDeletePsychologicalAssessment(id, isActive);

                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"Psychological assessment with ID {id} not found",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = $"Psychological assessment status updated to {(isActive ? "active" : "inactive")}",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error managing psychological assessment status",
                    Data = ex.Message
                });
            }
        }
    }
}
