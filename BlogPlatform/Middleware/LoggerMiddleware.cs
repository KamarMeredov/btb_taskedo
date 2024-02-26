namespace BlogPlatform.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            _logger.LogInformation("Request: {method} {path}", method, path);
            await _next(context);
            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("Response: {statusCode}", statusCode);
        }
    }
}
