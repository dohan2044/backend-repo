using Journey_of_faith.Application.exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Journey_of_faith.Api.middlewares
{
    public class GlobalExceptionHandler : Microsoft.AspNetCore.Diagnostics.IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
        {
            var statusCode = exception switch
            {
                BaseException exp => (int)exp.StatusCode,
                _ => StatusCodes.Status500InternalServerError,
            };

            if (exception is BaseException)
            {
                _logger.LogWarning(
                    exception,
                    "Request {Method} {Path} failed with status code {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode);
            }
            else
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
            }

            var problem = new ProblemDetails
            {
                Title = "Có lỗi sảy ra!!",
                Status = statusCode,
                Detail = exception is BaseException ba ? ba.Message : exception.Message,
                Instance = $"{context.Request.Method} - {context.Request.Path} - {context.Connection.RemoteIpAddress}"
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem);
            return true;
        }
    }
}
