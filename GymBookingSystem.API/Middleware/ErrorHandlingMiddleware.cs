using System.Text.Json;
using GymBookingSystem.API.Exceptions;

namespace GymBookingSystem.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case NotFoundException nfEx:
                context.Response.StatusCode = 404;
                response.Message = nfEx.Message;
                response.StatusCode = 404;
                break;

            case UnauthorizedException uEx:
                context.Response.StatusCode = 401;
                response.Message = uEx.Message;
                response.StatusCode = 401;
                break;

            case ForbiddenException fEx:
                context.Response.StatusCode = 403;
                response.Message = fEx.Message;
                response.StatusCode = 403;
                break;

            case BadRequestException brEx:
                context.Response.StatusCode = 400;
                response.Message = brEx.Message;
                response.StatusCode = 400;
                break;

            case ConflictException cEx:
                context.Response.StatusCode = 409;
                response.Message = cEx.Message;
                response.StatusCode = 409;
                break;

            case ValidationException vEx:
                context.Response.StatusCode = 422;
                response.Message = vEx.Message;
                response.StatusCode = 422;
                response.Errors = vEx.Errors;
                break;

            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode;
                response.Message = appEx.Message;
                response.StatusCode = appEx.StatusCode;
                break;

            default:
                context.Response.StatusCode = 500;
                response.Message = "An unexpected error occurred";
                response.StatusCode = 500;
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? Errors { get; set; }
    public string? TraceId { get; set; }
}
