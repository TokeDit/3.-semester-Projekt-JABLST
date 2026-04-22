
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration; // unecessary assignment, 
var services = builder.Services; // unecessary assignment, Did it to try to fix an issue.
// Add services to the container.
/*
services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddScoped<RepoMusicRecords>();
*/

// Jwt Authentication -----------------------------------------------------------------------------
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