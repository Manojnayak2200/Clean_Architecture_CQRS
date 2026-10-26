using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog; // 👈 Added for Logging
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURE SERILOG (Log to Console and File)
// ============================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/gateway-errors-.log",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning) // 👈 MUST BE WARNING
    .CreateLogger();

builder.Host.UseSerilog(); // Tell ASP.NET Core to use Serilog

// ============================================================
// 2. LOAD OCELOT CONFIG
// ============================================================
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// ============================================================
// 3. READ JWT SETTINGS
// ============================================================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ============================================================
// 4. 🛡️ REGISTER AUTHENTICATION
// ============================================================
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ============================================================
// 5. ADD OCELOT & SWAGGER
// ============================================================
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// ============================================================
// 6. 🟢 MIDDLEWARE PIPELINE (Order is critical!)
// ============================================================

// 1st: Catch ALL exceptions and log them
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2nd: Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 3rd: Swagger UI (MUST be before Ocelot)
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// 4th: Ocelot (Handles routing and forwarding)
await app.UseOcelot();

app.Run();