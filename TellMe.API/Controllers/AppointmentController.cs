using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Repository.Enums;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Appointment.AppointmentEndpoint)]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Create a new appointment
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Psychologist")]
        [ProducesResponseType(typeof(ResponseObject), 201)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            try
            {
                var expertId = Guid.Parse(User.FindFirst("UserId")?.Value!);
                var appointment = await _appointmentService.CreateAppointmentAsync(expertId, request);

                return Created($"api/v1/appointments/{appointment.Id}", new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Appointment created successfully",
                    Data = appointment
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
        /// Get all appointments based on user role
        /// </summary>
        [HttpGet("all")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> GetAllAppointments([FromQuery] Guid? userId, [FromQuery] Guid? expertId)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync(
                    userId: userId,
                    expertId: expertId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "All appointments retrieved successfully",
                    Data = appointments
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
        /// Get active appointments based on user role
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> GetActiveAppointments([FromQuery] Guid? userId, [FromQuery] Guid? expertId)
        {
            try
            {
                var appointments = await _appointmentService.GetActiveAppointmentsAsync(
                    userId: userId,
                    expertId: expertId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Active appointments retrieved successfully",
                    Data = appointments
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
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Appointment not found",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Appointment retrieved successfully",
                Data = appointment
            });
        }

        /// <summary>
        /// Update an appointment
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Psychologist")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                var updated = await _appointmentService.UpdateAppointmentAsync(id, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Appointment updated successfully",
                    Data = updated
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
        /// Cancel an appointment
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                var result = await _appointmentService.CancelAppointmentAsync(id);
                if (!result)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Failed to cancel appointment",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Appointment cancelled successfully",
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
        /// Update appointment status (Admin/Expert only)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] AppointmentStatus status)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                var updated = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Appointment status updated successfully",
                    Data = updated
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
