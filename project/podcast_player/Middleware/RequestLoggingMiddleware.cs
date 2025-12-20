using System.Diagnostics;

namespace Project.Middleware;

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
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var requestQueryString = context.Request.QueryString.ToString();
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        // Логируем входящий запрос
        _logger.LogInformation(
            "Входящий HTTP запрос: {Method} {Path}{QueryString} | IP: {ClientIp} | User-Agent: {UserAgent}",
            requestMethod,
            requestPath,
            requestQueryString,
            clientIp,
            userAgent);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            // Логируем результат запроса
            var logLevel = statusCode >= 500 ? LogLevel.Error :
                          statusCode >= 400 ? LogLevel.Warning :
                          LogLevel.Information;

            _logger.Log(
                logLevel,
                "HTTP запрос завершен: {Method} {Path}{QueryString} | Status: {StatusCode} | Время выполнения: {ElapsedMs}ms",
                requestMethod,
                requestPath,
                requestQueryString,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}

