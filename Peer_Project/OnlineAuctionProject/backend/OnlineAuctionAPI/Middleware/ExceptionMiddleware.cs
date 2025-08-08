using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AlreadyExistsException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NotFoundException ex)
        {
            var errors = new Dictionary<string, string[]> { { "email", new[] { ex.Message } } };
            await HandleExceptionAsync(
                context,
                StatusCodes.Status404NotFound,
                "Validation failed",
                errors
            );
        }
        catch (AlreadyDeletedException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ex.Message
            );
        }
        catch (NullValueException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (RepositoryOperationException ex)
        {
            if (ex.InnerException is NotFoundException notFoundEx)
            {
                await HandleExceptionAsync(
                    context,
                    StatusCodes.Status404NotFound,
                    notFoundEx.Message
                );
            }
            else
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += $" Inner exception: {ex.InnerException.Message}";
                }
                await HandleExceptionAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    message
                );
            }
        }
        catch (InvalidException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (InvalidDataException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred."
            );
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string message,
        object? errors = null
    )
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = new
        {
            success = false,
            message = message,
            data = (object)null,
            errors = errors,
        };
        await context.Response.WriteAsJsonAsync(response);
    }
}
