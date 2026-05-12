using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rest_SikkerApi.data;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// uddyber error msg på startup fejl, så man kan se hvad der gik galt, i stedet for en generisk "Application failed to start" besked. Det er især nyttigt under udvikling.
builder.WebHost.CaptureStartupErrors(true);
builder.WebHost.UseSetting("detailedErrors", "true");

var configuration = builder.Configuration; // unecessary assignment, 
var services = builder.Services; // unecessary assignment, Did it to try to fix an issue.
// Add services to the container.

services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DbConnection"), 
            sqlServerOptions =>
            {
                // Enable automatic retries for transient failures
                // Default: 6 retries with exponential backoff
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                );
            })); // looks in appSettings.json or environment variables for a connection string named "DefaultConnection"

// COMMIT 1: Register HttpClient via AddHttpClient to use IHttpClientFactory under the hood
// COMMIT 10: Register ITelegramService -> TelegramService for DI and testability
builder.Services.AddHttpClient<TelegramService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();

//  Register HttpClient for TelegramCommandHandler via IHttpClientFactory
// Register ITelegramCommandHandler -> TelegramCommandHandler for DI and testability
builder.Services.AddHttpClient<TelegramCommandHandler>();
builder.Services.AddScoped<ITelegramCommandHandler, TelegramCommandHandler>();

//services.AddScoped<RepoMusicRecords>();
// Register repository for database operations
builder.Services.AddScoped<SikkerRepo>();
builder.Services.AddScoped<ISikkerRepo, SikkerRepo>();

// Register background report service
builder.Services.AddHostedService<ReportService>();



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

//Register TelegramService and a chat registration store as singletons.
builder.Services.AddScoped(provider => 
    new TelegramBotService(
        telegramBotToken ?? string.Empty,
        provider.GetRequiredService<ISikkerRepo>()
    )
);

// builder.Services.AddHttpClient<IImageAnalysisService, GeminiImageAnalysisService>();
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
        // For testing: skip Firebase initialization if file doesn't exist
        try
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("firebase-service-account.json")
            });
        }
        catch (System.IO.FileNotFoundException)
        {
            // Skip Firebase setup for testing
            Console.WriteLine("Firebase service account file not found. Skipping Firebase initialization for testing.");
        }
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

// Middleware: verify Firebase ID token (if present) and store UID in HttpContext
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        var token = authHeader.Substring("Bearer ".Length).Trim();
        try
        {
            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            var uid = decoded.Uid;

            // make available to controllers/repos
            context.Items["FirebaseUid"] = uid;

            // attach as claim so existing auth/authorization can see it
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                // avoid duplicate claim
                if (!context.User.HasClaim(c => c.Type == "firebase_uid"))
                {
                    identity.AddClaim(new Claim("firebase_uid", uid));
                }
            }
            else
            {
                var newId = new ClaimsIdentity(new[] { new Claim("firebase_uid", uid) });
                context.User = new ClaimsPrincipal(newId);
            }
        }
        catch (Exception)
        {
            // Token invalid/expired => ignore or log. Do not throw here (leave to controllers/auth pipeline).
        }
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();

// Fallback to index.html so Vue Router handles all routes
// Must be AFTER MapControllers so API routes take priority
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();


