using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductsDb"));

// ✅ Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Google OAuth Configuration
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];

// ✅ Configure authentication with Google OAuth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
            
            ValidateAudience = true,
            ValidAudience = googleClientId,
            
            ValidateLifetime = true,
            
            ValidateIssuerSigningKey = false, // Google's keys are retrieved from Authority
            RequireSignedTokens = true,
            
            // Additional settings for better token handling
            RequireExpirationTime = true,
            ValidateTokenReplay = false
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // Check if token is in cookie
                string? token = context.Request.Cookies["id_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                Console.WriteLine($"Token validated for user: {email}");
                return Task.CompletedTask;
            }
        };
    });

// ✅ Enable Authorization Middleware
builder.Services.AddAuthorization();

// ✅ Add API Controllers
builder.Services.AddControllers();

var app = builder.Build();

// ✅ Enable CORS
app.UseCors("AllowFrontend");

// ✅ Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map API Controllers
app.MapControllers();

// ✅ Run the application
app.Run();