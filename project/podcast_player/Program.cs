using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Repositories;
using Project.Repositories.Interfaces;
using Project.Services;
using Project.Services.Interfaces;
using FluentValidation;
using Project.Validators;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Project.Middleware;
using Prometheus;
using Project.Constants;

// Загрузка переменных окружения из .env файла
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Добавление переменных окружения в конфигурацию
builder.Configuration.AddEnvironmentVariables();

// Установка ApiKey из переменной окружения в конфигурацию
var apiKey = Environment.GetEnvironmentVariable("API_KEY");
if (!string.IsNullOrEmpty(apiKey))
{
    builder.Configuration["ApiKey"] = apiKey;
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Podcast Player API",
        Version = "v1",
        Description = "API для управления подкастами, эпизодами, плейлистами и категориями",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Podcast Player",
            Email = "support@podcastplayer.com"
        }
    });
    
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key авторизация. Используйте заголовок X-API-Key",
        Name = "X-API-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityDefinition("UserId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "ID пользователя. Используйте заголовок X-User-Id (опционально, по умолчанию 0)",
        Name = "X-User-Id",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "UserIdScheme"
    });

    c.AddSecurityDefinition("UserRole", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Роль пользователя: User, Manager, Admin. Используйте заголовок X-User-Role (опционально, по умолчанию User)",
        Name = "X-User-Role",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "UserRoleScheme"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        },
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "UserId"
                }
            },
            Array.Empty<string>()
        },
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "UserRole"
                }
            },
            Array.Empty<string>()
        }
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Настройка аутентификации и авторизации
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, Project.Middleware.ApiKeyAuthenticationHandler>("ApiKey", options => { });

builder.Services.AddAuthorization(options =>
{
    Project.Authorization.AuthorizationPolicies.ConfigurePolicies(options);
});

builder.Services.AddHttpContextAccessor();

// Настройка Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(ErrorMessages.Connection.ConnectionFailed);
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Настройка Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString);
            configuration.AbortOnConnectFail = false;
            configuration.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(configuration);
        });
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });
    }
    catch
    {
        // Redis не обязателен для работы приложения
    }
}

// Регистрация репозиториев
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPodcastRepository, PodcastRepository>();
builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Регистрация сервисов
builder.Services.AddScoped<IPodcastService, PodcastService>();
builder.Services.AddScoped<IEpisodeService, EpisodeService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<Project.Services.IAuthorizationService, Project.Services.AuthorizationService>();

// Настройка FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<PodcastValidator>();

// Настройка AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Настройка Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks()
    .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API доступен"))
    .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "postgresql" }, timeout: TimeSpan.FromSeconds(3));

// Добавляем Redis health check только если Redis настроен
if (!string.IsNullOrEmpty(redisConnectionString))
{
    healthChecksBuilder.AddRedis(redisConnectionString, name: "redis", tags: new[] { "cache", "redis" }, timeout: TimeSpan.FromSeconds(3));
}

var app = builder.Build();

// Автоматическое создание базы данных при старте
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Проверка и создание базы данных...");
    dbContext.Database.EnsureCreated();
    logger.LogInformation("База данных готова к работе.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ошибка при создании базы данных: {Message}", ex.Message);
    logger.LogWarning("Продолжаем работу. Убедитесь, что база данных создана вручную или выполните SQL скрипт create_tables.sql");
}

// Глобальный обработчик ошибок (должен быть первым)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Логирование HTTP-запросов
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

// Health Check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("cache")
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Name == "api" // Только проверка доступности API
});

app.Run();