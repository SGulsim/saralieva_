using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Project.Models;

namespace Project.Middleware;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string API_KEY_HEADER = "X-API-Key";
    private const string USER_ID_HEADER = "X-User-Id";
    private const string USER_ROLE_HEADER = "X-User-Role";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Пропускаем аутентификацию для публичных эндпоинтов
        if (Request.Path.StartsWithSegments("/swagger") ||
            Request.Path.StartsWithSegments("/api/auth") ||
            Request.Path.StartsWithSegments("/metrics") ||
            Request.Path.StartsWithSegments("/health") ||
            Request.Path.Value?.EndsWith("/swagger.json", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeaderValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key отсутствует"));
        }

        var apiKey = apiKeyHeaderValues.ToString();
        if (string.IsNullOrEmpty(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key пуст"));
        }

        // Проверяем API ключ
        var validApiKey = Environment.GetEnvironmentVariable("API_KEY") 
            ?? _configuration["ApiKey"] 
            ?? "default-api-key-12345";

        if (apiKey != validApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Неверный API Key"));
        }

        // Получаем информацию о пользователе из заголовков
        var userId = Request.Headers[USER_ID_HEADER].ToString();
        var userRole = Request.Headers[USER_ROLE_HEADER].ToString();

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
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, userRole),
            new Claim("UserId", userId),
            new Claim("Role", userRole),
            new Claim("ApiKey", apiKey)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

