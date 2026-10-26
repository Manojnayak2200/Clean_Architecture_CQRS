using System.Net;
using System.Text.Json;
using Serilog;

namespace ApiGateway.Middleware
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
                // Let Ocelot process the request and get the response from the microservice
                await _next(context);

                // 🟢 LOG EVERYTHING EXCEPT 200 OK
                if (context.Response.StatusCode != 200)
                {
                    // Skip Swagger UI logs to keep it clean
                    if (!context.Request.Path.StartsWithSegments("/swagger"))
                    {
                        // Since this is a non-200 response (like 400 Bad Request), we use Warning level
                        Log.Warning("Gateway returned {StatusCode} for {Method} {Path}. TraceId: {TraceId}",
                            context.Response.StatusCode,
                            context.Request.Method,
                            context.Request.Path,
                            context.TraceIdentifier);
                    }
                }
            }
            catch (Exception ex)
            {
                // 🔴 CATCH GATEWAY EXCEPTIONS (e.g., cannot connect to service)
                Log.Error(ex, "Unhandled exception at Gateway. Path: {Path}, TraceId: {TraceId}",
                    context.Request.Path, context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                statusCode = 500,
                message = "An unexpected error occurred at the Gateway. Please try again later.",
                traceId = context.TraceIdentifier
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}