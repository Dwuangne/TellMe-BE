using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class PatientProfileController : ControllerBase
    {
        private readonly IPatientProfileService _patientProfileService;

        public PatientProfileController(IPatientProfileService patientProfileService)
        {
            _patientProfileService = patientProfileService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPatientProfiles([FromQuery] Guid? userId = null)
        {
            try
            {
                var profiles = await _patientProfileService.GetAllPatientProfilesAsync(userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Patient profiles retrieved successfully",
                    Data = profiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving patient profiles",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("expert")]
        [Authorize(Roles = "Admin, Expert")]
        public async Task<IActionResult> GetAllPatientProfilesForExpert([FromQuery] Guid? expertId = null)
        {
            try
            {
                var profiles = await _patientProfileService.GetAllActivePatientProfilesAsyncForExpert(expertId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Patient profiles for expert retrieved successfully",
                    Data = profiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving patient profiles for expert",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetPatientProfile(Guid userId)
        {
            try
            {
                var profile = await _patientProfileService.GetPatientProfileAsync(userId);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Patient profile retrieved successfully",
                    Data = profile
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Patient profile not found",
                    Data = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error retrieving patient profile",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreatePatientProfile([FromBody] CreatePatientProfileRequest request)
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

                var profile = await _patientProfileService.CreatePatientProfileAsync(request);
                var response = new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Patient profile created successfully",
                    Data = profile
                };

                return CreatedAtAction(nameof(GetPatientProfile), new { userId = profile.UserId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error creating patient profile",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdatePatientProfile(int id, [FromBody] UpdatePatientProfileRequest request)
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

                var profile = await _patientProfileService.UpdatePatientProfileAsync(id, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Patient profile updated successfully",
                    Data = profile
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Patient profile not found",
                    Data = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error updating patient profile",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult ManageDeletePatientProfile(int id, [FromQuery] bool isActive = false)
        {
            try
            {
                var result = _patientProfileService.ManageDeletePatientProfile(id, isActive);
                if (result)
                {
                    return Ok(new ResponseObject
                    {
                        Status = HttpStatusCode.OK,
                        Message = $"Patient profile status updated to {(isActive ? "active" : "inactive")}",
                        Data = result
                    });
                }
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"Patient profile with ID {id} not found",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error managing patient profile status",
                    Data = ex.Message
                });
            }
        }
    }
}
