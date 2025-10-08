using System.Text.Json;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Middleware
{   
    public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, "Validation failed"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict occurred"),
            _ => (StatusCodes.Status500InternalServerError, "Server error")
        };

        var errors = new Dictionary<string, string[]>
        {
            { "general", new[] {
                exception is BadRequestException or NotFoundException or UnauthorizedException or ConflictException
                    ? exception.Message
                    : "An unexpected error occurred."
            } }
        };

        var response = new
        {
            success = false,
            message = title,
            data = (object)null,
            errors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

    
}