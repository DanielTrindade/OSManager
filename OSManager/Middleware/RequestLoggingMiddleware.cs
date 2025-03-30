using System.Diagnostics;

namespace OSManager.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // Log de início da requisição
            var requestId = Guid.NewGuid().ToString();
            var requestMethod = context.Request.Method;
            var requestPath = context.Request.Path;
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var userIP = context.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                "Request {RequestId} started: {RequestMethod} {RequestPath} | IP: {UserIP} | Agent: {UserAgent}",
                requestId, requestMethod, requestPath, userIP, userAgent);

            // Tempo da requisição
            var timer = Stopwatch.StartNew();

            try
            {
                await _next(context);

                timer.Stop();

                // Log de finalização
                _logger.LogInformation(
                    "Request {RequestId} completed: {StatusCode} in {ElapsedMs}ms",
                    requestId, context.Response.StatusCode, timer.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                timer.Stop();

                // Log de erro
                _logger.LogError(
                    ex,
                    "Request {RequestId} failed: {Message} in {ElapsedMs}ms",
                    requestId, ex.Message, timer.ElapsedMilliseconds);

                throw; // Repassar a exceção para ser tratada pelo middleware de erros
            }
        }
    }

    // Extension method para facilitar registro
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}