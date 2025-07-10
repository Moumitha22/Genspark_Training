using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);

            var (statusCode, title) = context.Exception switch
            {
                BadRequestException => (400, "Validation failed"),
                UnauthorizedException => (401, "Unauthorized"),
                NotFoundException => (404, "Resource not found"),
                ConflictException => (409, "Conflict occurred"),
                _ => (500, "Server error")
            };

            var errors = new Dictionary<string, string[]>
            {
                { "general", new[] {
                    context.Exception is BadRequestException or NotFoundException or UnauthorizedException or ConflictException
                        ? context.Exception.Message
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

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }
    }
}
