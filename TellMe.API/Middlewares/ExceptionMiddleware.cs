using System.Text.Json;
using TellMe.Service.Errors;
using TellMe.Service.Exceptions;

namespace TellMe.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request: {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                NotFoundException nf => (StatusCodes.Status404NotFound, nf.Message),
                BadRequestException br => (StatusCodes.Status400BadRequest, br.Message),
                ConflictException cf => (StatusCodes.Status409Conflict, cf.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var error = new Error
            {
                StatusCode = statusCode,
                Message = TryParseErrorDetails(ex.Message) ?? new List<ErrorDetail> { new ErrorDetail { Message = message } }
            };

            // Sử dụng System.Text.Json để serialize
            await context.Response.WriteAsync(error.ToString());
        }

        private static List<ErrorDetail>? TryParseErrorDetails(string message)
        {
            try
            {
                return JsonSerializer.Deserialize<List<ErrorDetail>>(message);
            }
            catch (JsonException ex)
            {
                // Log lỗi parsing nếu cần
                return null;
            }
        }
    }
}
