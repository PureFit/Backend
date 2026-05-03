using System.Text;
using System.Text.Json.Serialization;
using Backend.Application.Common;
using Backend.Application.Repositories;
using Backend.Application.Services;
using Backend.Application.Repositories;
using Backend.Application.Services.impl;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.ExternalApi;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));

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

builder.Services.Configure<ExerciseApiSettings>(builder.Configuration.GetSection("ExerciseApi"));
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IExternalExerciseRepository, ExerciseApiClient>();
builder.Services.AddHttpClient<ExerciseApiClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
