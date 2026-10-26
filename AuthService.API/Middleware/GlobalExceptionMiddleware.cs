using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace AuthService.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception in Auth Service. Path: {Path}, TraceId: {TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted) return Task.CompletedTask;

            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                // 🟢 Business rule: "Username already taken", "Invalid credentials"
                case InvalidOperationException:
                    context.Response.StatusCode = 400;
                    response.StatusCode = 400;
                    response.Message = exception.Message;
                    break;

                // 🟢 Unauthorized: wrong password
                case UnauthorizedAccessException:
                    context.Response.StatusCode = 401;
                    response.StatusCode = 401;
                    response.Message = exception.Message;
                    break;

                // 🟢 Not found
                case KeyNotFoundException:
                    context.Response.StatusCode = 404;
                    response.StatusCode = 404;
                    response.Message = exception.Message;
                    break;

                // 🟢 Duplicate user (unique constraint violation)
                case DbUpdateException dbEx
                    when dbEx.InnerException?.Message.Contains("Duplicate entry") == true:
                    context.Response.StatusCode = 409;
                    response.StatusCode = 409;
                    response.Message = "This username already exists. Please check your input.";
                    break;

                // 🟢 Any DB error
                case DbUpdateException:
                    context.Response.StatusCode = 500;
                    response.StatusCode = 500;
                    response.Message = "A database error occurred. Please try again later.";
                    break;

                // 🟢 Fallback
                default:
                    context.Response.StatusCode = 500;
                    response.StatusCode = 500;
                    response.Message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TraceId { get; set; }
    }
}
