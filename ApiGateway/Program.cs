using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.SwaggerForOcelot.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;    // 👈 ADD THIS (for QoSOptions)
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. SERILOG (Console + File with ABSOLUTE path)
// ============================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "Logs", "gateway-errors-.log"),   // 👈 ABSOLUTE PATH
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================================
// 2. LOAD OCELOT CONFIG
// ============================================================
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// ============================================================
// 3. JWT SETTINGS
// ============================================================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ============================================================
// 4. AUTHENTICATION
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
// 5. OCELOT + POLLY + SWAGGER
// ============================================================
builder.Services.AddOcelot(builder.Configuration).AddPolly();   // 👈 .AddPolly() added
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// ============================================================
// 6. MIDDLEWARE PIPELINE
// ============================================================
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();

app.Run();