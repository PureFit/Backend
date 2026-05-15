using Backend.Application.Common;
using Backend.Application.Repositories;
using Backend.Application.Services;
using Backend.Application.Services.impl;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/purefit_log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));
builder.Services.Configure<SvgSettings>(builder.Configuration.GetSection("SvgSettings"));
builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("RedisConfig"));
builder.Services.Configure<MuscleCalculatorConfig>(builder.Configuration.GetSection("MuscleCalculatorConfig"));
builder.Services.Configure<ExerciseParameterConfig>(builder.Configuration.GetSection("ExerciseParameterConfig"));

var redisConnectionString = builder.Configuration["RedisConfig:ConnectionString"]!;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = builder.Configuration["RedisConfig:KeyPrefix"];
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!))
        };
    });
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IImageOrchestratorService, ImageOrchestratorService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();
builder.Services.AddScoped<IUserMetricLogRepository, UserMetricLogRepository>();
builder.Services.AddScoped<IImageRetrieverService, CloudinaryService>();
builder.Services.AddScoped<IUserGoogleTokenRepository, UserGoogleTokenRepository>();
builder.Services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
builder.Services.AddHttpClient<GoogleOAuthService>();

builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();

builder.Services.AddScoped<ISvgManipulationService, SvgManipulationService>();
builder.Services.AddScoped<IMuscleVisualizationService, MuscleVisualizationService>();

builder.Services.AddScoped<ITrainingSetRepository, TrainingSetRepository>();
builder.Services.AddScoped<ITrainingSetService, TrainingSetService>();
builder.Services.AddScoped<IMuscleCalculatorService, MuscleCalculatorService>();

builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection("GroqSettings"));
builder.Services.AddHttpClient<IAIClient, GroqClient>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IPromptBuilder, PromptBuilder>();
builder.Services.AddScoped<ICorePromptEnricher, CorePromptEnricher>();
builder.Services.AddScoped<IPlanGenerator, PlanGenerator>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanScheduler, PlanScheduler>();
builder.Services.AddScoped<IPlanService, PlanService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
