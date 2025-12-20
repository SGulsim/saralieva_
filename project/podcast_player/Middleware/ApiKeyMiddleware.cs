using System.Security.Claims;
using Project.Models;

namespace Project.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private const string API_KEY_HEADER = "X-API-Key";
    private const string USER_ID_HEADER = "X-User-Id";
    private const string USER_ROLE_HEADER = "X-User-Role";
    private const string API_KEY_CLAIM = "ApiKey";

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        var apiKey = Environment.GetEnvironmentVariable("API_KEY") 
            ?? configuration["ApiKey"] 
            ?? "default-api-key-12345";

        if (context.Request.Path.StartsWithSegments("/swagger") || 
            context.Request.Path.StartsWithSegments("/api/auth") ||
            context.Request.Path.StartsWithSegments("/metrics") ||
            context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.Value?.EndsWith("/swagger.json", StringComparison.OrdinalIgnoreCase) == true)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            _logger.LogWarning(
                "Ошибка авторизации: API Key отсутствует | Path: {Path} | Method: {Method} | IP: {Ip}",
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress?.ToString());
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key отсутствует. Используйте заголовок X-API-Key");
            return;
        }

        var apiKeyValue = extractedApiKey.ToString();
        if (string.IsNullOrEmpty(apiKeyValue) || apiKeyValue != apiKey)
        {
            _logger.LogWarning(
                "Ошибка авторизации: Неверный API Key | Path: {Path} | Method: {Method} | IP: {Ip}",
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress?.ToString());
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Неверный API Key");
            return;
        }

        // Получаем информацию о пользователе из заголовков
        var userId = context.Request.Headers[USER_ID_HEADER].ToString();
        var userRole = context.Request.Headers[USER_ROLE_HEADER].ToString();

        // Если роль не указана, используем User по умолчанию
        if (string.IsNullOrEmpty(userRole))
        {
            userRole = Role.User.ToString();
        }

        // Если ID пользователя не указан, используем 0
        if (string.IsNullOrEmpty(userId))
        {
            userId = "0";
        }

        var claims = new List<Claim>
        {
            new Claim(API_KEY_CLAIM, apiKeyValue),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, userRole),
            new Claim("UserId", userId),
            new Claim("Role", userRole)
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }
}

