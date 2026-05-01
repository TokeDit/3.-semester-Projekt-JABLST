using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Rest_SikkerApi.data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Rest_SikkerApi.repos;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Rest_SikkerApi.Services;

var builder = WebApplication.CreateBuilder(args);
// uddyber error msg på startup fejl, så man kan se hvad der gik galt, i stedet for en generisk "Application failed to start" besked. Det er især nyttigt under udvikling.
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseSetting("detailedErrors", "true");

var configuration = builder.Configuration; // unecessary assignment, 
var services = builder.Services; // unecessary assignment, Did it to try to fix an issue.
// Add services to the container.

services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // looks in appSettings.json or environment variables for a connection string named "DefaultConnection"
//services.AddScoped<RepoMusicRecords>();
// Register repository for database operations
builder.Services.AddScoped<SikkerRepo>();

// ==========================================================================================
// Telegram Bot Configuration
// ==========================================================================================
// The TelegramService is configured here with credentials:
// - Bot Token: The unique token for your Telegram bot (create via @BotFather)
// - Chat ID: The Telegram chat where messages will be sent
//
// Configuration sources (in order of precedence):
// 1. Environment variables: TELEGRAM_BOT_TOKEN, TELEGRAM_CHAT_ID
// 2. appsettings.json: "Telegram" section with "BotToken" and "ChatId"
// 3. Hardcoded default token in TelegramService class
//
// Example appsettings.json configuration:
// {
//   "Telegram": {
//     "BotToken": "your-bot-token-here",
//     "ChatId": "your-chat-id-here"
//   }
// }
// ==========================================================================================

var telegramSection = builder.Configuration.GetSection("Telegram");
var telegramBotToken = telegramSection["BotToken"] ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var telegramChatId = telegramSection["ChatId"] ?? Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");

// Register TelegramService as a singleton (one instance for the entire application)
builder.Services.AddSingleton(new TelegramService(telegramBotToken ?? string.Empty, telegramChatId ?? string.Empty));

builder.Services.AddHttpClient<IImageAnalysisService, GeminiImageAnalysisService>();
// Jwt Authentication -----------------------------------------------------------------------------
// ==========================================================================================
// JWT Authentication Configuration
// ==========================================================================================
// Configure JWT token validation for API endpoints
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// hvad forventer vi os i tokenet, og hvordan validerer vi det?
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


// CORS - Cross-Origin Resource Sharing ---------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowAll",
        policy =>
        {

            policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Swagger - API documentation ------------------------------------------------------
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token as: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey, // Vi bruger ApiKey for at kunne skrive 'Bearer ' manuelt
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Kravet om at bruge ovenstående definition på endpoints
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});
// Firebase Admin SDK Setup
if (FirebaseApp.DefaultInstance is null)
{
    var firebaseCredentialsJson = builder.Configuration["Firebase:ServiceAccountJson"];

    if (string.IsNullOrWhiteSpace(firebaseCredentialsJson))
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("firebase-service-account.json")
        });
    }
    else
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseCredentialsJson)
        });
    }
}

// Repositories and Controllers ------------------------------------------------------
//builder.Services.AddSingleton<>();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
// Remove if you want swagger in production
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
//}

app.UseCors("allowAll");

// Serve the Vue frontend from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication(); // Checks "Who are you?"
app.UseAuthorization();
app.MapControllers();

// Fallback to index.html so Vue Router handles all routes
// Must be AFTER MapControllers so API routes take priority
app.MapFallbackToFile("index.html");

app.Run();