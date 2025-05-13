using System.Net;
using System.Text.Json;
using TellMe.Service.Exceptions;
using TellMe.Service.Models;

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
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
            }
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

        var response = new ResponseObject
        {
            Data = null
        };

        switch (ex)
        {
            case NotFoundException nf:
                response.Status = HttpStatusCode.NotFound;
                response.Message = nf.Message;
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case BadRequestException br:
                response.Status = HttpStatusCode.BadRequest;
                response.Message = br.Message;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case ConflictException cf:
                response.Status = HttpStatusCode.Conflict;
                response.Message = cf.Message;
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                break;

            case UnauthorizedAccessException _:
                response.Status = HttpStatusCode.Forbidden;
                response.Message = "You do not have permission to access this resource.";
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                break;

            default:
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Đã xảy ra lỗi không mong muốn.";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}