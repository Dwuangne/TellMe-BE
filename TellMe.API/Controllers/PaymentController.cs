using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TellMe.API.Constants;
using TellMe.Repository.Enums;
using TellMe.Service.Exceptions;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.API.Controllers
{
    [Route(APIEndPointConstant.Payment.PaymentEndpoint)]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVnPayService _vnPayService;
        private readonly IPayOsService _payOsService;
        private readonly string paymentSuccessUrl = "http://localhost:5173/payment/success";
        private readonly string paymentFailUrl = "http://localhost:5173/payment/fail";

        public PaymentController(IPaymentService paymentService, IVnPayService vnPayService, IPayOsService payOsService)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
            _payOsService = payOsService;
        }

        [HttpPost]
        [Authorize]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePaymentUrlVnpay(CreatePaymentRequest model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Payment request cannot be null",
                        Data = null
                    });
                }

                if (!model.IsValid())
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Invalid payment request. Either AppointmentId or UserSubscriptionId must be provided, but not both.",
                        Data = null
                    });
                }

                var url = await _vnPayService.CreatePaymentUrl(model, HttpContext);

                if (string.IsNullOrEmpty(url))
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Failed to create payment URL",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully created payment URL",
                    Data = url
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred while processing your payment request",
                    Data = null
                });
            }
        }

        [HttpGet]
        //[Authorize]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                var txnRef = HttpContext.Request.Query["vnp_TxnRef"].ToString();

                if (string.IsNullOrWhiteSpace(txnRef) || !txnRef.Contains('-'))
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Transaction reference is invalid or missing",
                        Data = null
                    });
                }

                var paymentIdString = txnRef.Split('|')[0];

                var response = await _vnPayService.PaymentExecute(Request.Query, paymentIdString);

                if (response == null)
                {
                    //return BadRequest(new ResponseObject
                    //{
                    //    Status = HttpStatusCode.BadRequest,
                    //    Message = "Invalid payment response",
                    //    Data = null
                    //});
                    return Redirect(paymentFailUrl);
                }

                if (response.Status == PaymentStatus.Success)
                {
                    //return Ok(new ResponseObject
                    //{
                    //    Status = HttpStatusCode.OK,
                    //    Message = "Payment processed successfully",
                    //    Data = response
                    //});
                    return Redirect(paymentSuccessUrl);
                }
                else
                {
                    // 402 Payment Required for failed payment
                    //return StatusCode((int)HttpStatusCode.PaymentRequired, new ResponseObject
                    //{
                    //    Status = HttpStatusCode.PaymentRequired,
                    //    Message = "Payment failed",
                    //    Data = response
                    //});
                    return Redirect(paymentFailUrl);
                }
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

        [HttpPost("payos")]
        [Authorize]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePaymentUrlPayOs(CreatePaymentRequest model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Payment request cannot be null",
                        Data = null
                    });
                }

                if (!model.IsValid())
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Invalid payment request. Either AppointmentId or UserSubscriptionId must be provided, but not both.",
                        Data = null
                    });
                }

                var url = await _payOsService.CreatePaymentUrl(model, HttpContext);

                if (string.IsNullOrEmpty(url))
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Failed to create payment URL",
                        Data = null
                    });
                }

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully created PayOS payment URL",
                    Data = url
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred while processing your payment request",
                    Data = null
                });
            }
        }

        [HttpGet("return_url")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> PayOsReturnUrl()
        {
            try
            {

                var response = await _payOsService.PaymentExecute(Request.Query);

                if (response == null)
                {
                    return BadRequest(new ResponseObject
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Invalid payment response",
                        Data = null
                    });
                }

                if (response.Status == PaymentStatus.Success)
                {
                    // return Ok(new ResponseObject
                    // {
                    //     Status = HttpStatusCode.OK,
                    //     Message = "Payment processed successfully",
                    //     Data = response
                    // });
                    return Redirect(paymentSuccessUrl);
                }
                else
                {
                    // 402 Payment Required for failed payment
                    // return StatusCode((int)HttpStatusCode.PaymentRequired, new ResponseObject
                    // {
                    //     Status = HttpStatusCode.PaymentRequired,
                    //     Message = "Payment failed",
                    //     Data = response
                    // });
                    return Redirect(paymentFailUrl);
                }
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

        [HttpGet("cancel_url")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        public IActionResult PayOsCancelUrl([FromQuery] string paymentId)
        {
            // return Ok(new ResponseObject
            // {
            //     Status = HttpStatusCode.OK,
            //     Message = "Payment was cancelled by user.",
            //     Data = null
            // });
            return Redirect(paymentFailUrl);
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{id}/payment")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Payment not found",
                    Data = null
                });
            }

            // Verify the user has access to this payment
            //var userId = Guid.Parse(User.FindFirst("UserId")?.Value ?? string.Empty);
            //if (payment.UserId != userId && !User.IsInRole("Admin"))
            //{
            //    return Forbid();
            //}

            return Ok(new ResponseObject
            {
                Status = HttpStatusCode.OK,
                Message = "Successfully retrieved payment",
                Data = payment
            });
        }

        /// <summary>
        /// Get current user's payments
        /// </summary>
        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetUserPayments()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("UserId")?.Value ?? string.Empty);
                var payments = await _paymentService.GetUserPaymentsAsync(userId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully retrieved user payments",
                    Data = payments
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
        /// Get all payments (Admin only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPayments([FromQuery] Guid? userId = null)
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync(userId);

                return Ok(new ResponseObject
                {
                    Status = HttpStatusCode.OK,
                    Message = userId.HasValue
                        ? $"Successfully retrieved all payments for user {userId}"
                        : "Successfully retrieved all payments",
                    Data = payments
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseObject
                {
                    Status = HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (BadRequestException ex)
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseObject
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving payments",
                    Data = null
                });
            }
        }

    }
}
