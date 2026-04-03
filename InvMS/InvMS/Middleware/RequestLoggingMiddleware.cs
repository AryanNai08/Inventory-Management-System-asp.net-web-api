using System.Diagnostics;

namespace InvMS.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation("REQUEST  HTTP {method} {path} incoming", method, path);

            Stopwatch stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var elapsedTime = stopwatch.ElapsedMilliseconds;

            if (statusCode < 400)
            {
                _logger.LogInformation("RESPONSE HTTP {method} {path} responded {statusCode} in {elapsedTime}ms", method, path, statusCode, elapsedTime);
            }
            else if (statusCode >= 400 && statusCode < 500)
            {
                _logger.LogWarning("RESPONSE HTTP {method} {path} responded {statusCode} in {elapsedTime}ms", method, path, statusCode, elapsedTime);
            }
            else if (statusCode >= 500)
            {
                _logger.LogError("RESPONSE HTTP {method} {path} responded {statusCode} in {elapsedTime}ms", method, path, statusCode, elapsedTime);
            }
        }
    }
}
