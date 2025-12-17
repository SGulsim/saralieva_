using Microsoft.EntityFrameworkCore;
using Project.data;
using Project.Repositories;
using Project.Repositories.Interfaces;
using Project.Services;
using Project.Services.Interfaces;
using FluentValidation;
using Project.Validators;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Настройка Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("коннекшн не удался");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Настройка Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = ConfigurationOptions.Parse(redisConnectionString);
        return ConnectionMultiplexer.Connect(configuration);
    });
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
    });
}

// Регистрация репозиториев
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPodcastRepository, PodcastRepository>();

// Регистрация сервисов
builder.Services.AddScoped<IPodcastService, PodcastService>();

// Настройка FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<PodcastValidator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();