using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TellMe.API.Constants;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.PsychologicalTest.PsychologicalTestEndpoint)]
    [ApiController]
    public class PsychologicalTestController : ControllerBase
    {
        private readonly IPsychologicalTestService _psychologicalTestService;

        public PsychologicalTestController(IPsychologicalTestService psychologicalTestService)
        {
            _psychologicalTestService = psychologicalTestService;
        }

        /// <summary>
        /// Get all psychological tests
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive tests</param>
        /// <returns>List of psychological tests</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        public async Task<IActionResult> GetAllTests([FromQuery] bool includeInactive = false)
        {
            var tests = await _psychologicalTestService.GetAllTestsAsync(includeInactive);
            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved all tests",
                Data = tests
            });
        }

        /// <summary>
        /// Get a psychological test by id
        /// </summary>
        /// <param name="id">Test id</param>
        /// <returns>Psychological test</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetTestById(Guid id)
        {
            try
            {
                var test = await _psychologicalTestService.GetTestByIdAsync(id);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved test",
                    Data = test
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

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetTestQuestions([FromBody] CreatePsychologicalTestRequest request)
        {
            var result = await _psychologicalTestService.CreateTestAsync(request);

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Create successfull psychological test",
                Data = result
            });  
        }



        /// <summary>
        /// Update an existing psychological test
        /// </summary>
        /// <param name="id">Test id</param>
        /// <param name="request">Test update request</param>
        /// <returns>Updated test</returns>
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> UpdateTest(Guid id, [FromBody] UpdatePsychologicalTestRequest request)
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
                var updatedTest = await _psychologicalTestService.UpdateTestAsync(id, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Test updated successfully",
                    Data = updatedTest
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
        /// Soft delete a psychological test
        /// </summary>
        /// <param name="id">Test id</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 204)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> DeleteTest(Guid id)
        {
            var deleted = await _psychologicalTestService.SoftDeleteTestAsync(id);
            if (!deleted)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"Not found pyschological test with {id}",
                    Data = null
                });
            }

            return Ok( new ResponseObject
            {
                Status = HttpStatusCode.NoContent,
                Message = "Test deleted successfully",
                Data = null
            });
        }
    }
}