using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.SubscriptionPackage.SubscriptionPackageEndpoint)]
    [ApiController]
    public class SubscriptionPackageController : ControllerBase
    {
        private readonly ISubscriptionPackageService _packageService;

        public SubscriptionPackageController(ISubscriptionPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Get all subscription packages with optional inclusion of inactive ones
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved all packages",
                    Data = packages
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
        /// Get all active subscription packages
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetActivePackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesActiveAsync();
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved active packages",
                    Data = packages
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
        /// Get a specific subscription package by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Package not found",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved package",
                Data = package
            });
        }

        /// <summary>
        /// Create a new subscription package, DurationUnit Day - Month - Year
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 201)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageRequest request)
        {
            try
            {
                var package = await _packageService.CreatePackageAsync(request);
                return Created($"api/v1/subscription-packages/{package.Id}", new ResponseObject
                {
                    Status = HttpStatusCode.Created,
                    Message = "Package created successfully",
                    Data = package
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
        /// Update an existing subscription package, DurationUnit Day - Month - Year
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageRequest request)
        {
            try
            {
                var package = await _packageService.UpdatePackageAsync(id, request);
                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Package updated successfully",
                    Data = package
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
        }

        /// <summary>
        /// Delete (deactivate) a subscription package
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                var result = await _packageService.DeletePackageAsync(id);
                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Package not found or already deleted",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Package deleted successfully",
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
        /// Restore a deleted subscription package
        /// </summary>
        [HttpPut("{id}/restore")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> RestorePackage(int id)
        {
            try
            {
                var result = await _packageService.RestorePackageAsync(id);
                if (!result)
                {
                    return NotFound(new ResponseObject
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = "Package not found or already active",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Package restored successfully",
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
    }
}
