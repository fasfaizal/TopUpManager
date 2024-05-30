using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace TopUpManager.API.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, $"Error: {exception.Message}");

            // Create custom response
            var details = new ProblemDetails()
            {
                Title = "API Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Type = exception.GetType().Name,
                Detail = exception.Message
            };
            var response = JsonSerializer.Serialize(details);
            httpContext.Response.ContentType = "application/json";

            // Add the custom response to http response
            await httpContext.Response.WriteAsync(response, cancellationToken);
            return true;
        }
    }
}
