using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TellMe.API.Constants;
using TellMe.API.Helper;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Psychologist.PsychologistEndpoint)]
    [ApiController]
    public class PsychologistController : ControllerBase
    {
        private readonly IPsychologicalService _psychologistService;

        public PsychologistController(IPsychologicalService psychologistService)
        {
            _psychologistService = psychologistService;
        }

        /// <summary>
        /// Get all psychologists
        /// </summary>
        /// <returns>List of psychologists</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPsychologists()
        {
            var psychologists = await _psychologistService.GetAllPsychologistsAsync();
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved all psychologists",
                Data = psychologists
            });
        }

        /// <summary>
        /// Get a psychologist by ID
        /// </summary>
        /// <param name="id">Psychologist ID</param>
        /// <returns>Psychologist details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPsychologistById(Guid id)
        {
            try
            {
                var psychologist = await _psychologistService.GetPsychologistByIdAsync(id);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved psychologist",
                    Data = psychologist
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

        /// <summary>
        /// Create a new psychologist
        /// </summary>
        /// <param name="request">Psychologist create request</param>
        /// <returns>Created psychologist</returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePsychologist([FromBody] PsychologistCreateRequest request)
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

            var result = await _psychologistService.CreatePsychologistAsync(request);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Psychologist created successfully",
                Data = result
            });
        }

        /// <summary>
        /// Update an existing psychologist
        /// </summary>
        /// <param name="id">Psychologist ID</param>
        /// <param name="request">Psychologist update request</param>
        /// <returns>Updated psychologist</returns>
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Psychologist")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePsychologist(Guid id, [FromBody] PsychologistUpdateRequest request)
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

            try
            {
                var updatedPsychologist = await _psychologistService.UpdatePsychologistAsync(id, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Psychologist updated successfully",
                    Data = updatedPsychologist
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
    }
} 