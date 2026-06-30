using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Data;
using backend.Models.Core;
using System.Security.Claims;

namespace backend.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Enable buffering so the request body can be read multiple times (e.g., by controllers and then here)
                context.Request.EnableBuffering();
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? context.User?.FindFirst("id")?.Value;
                
                var username = context.User?.FindFirst(ClaimTypes.Name)?.Value 
                            ?? context.User?.FindFirst("username")?.Value;

                var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                // Read query string
                string requestParameter = context.Request.QueryString.Value ?? "";
                
                // Read request body if buffering was enabled and stream is seekable
                if (context.Request.Body.CanSeek)
                {
                    context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                    using (var reader = new System.IO.StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true, 1024, true))
                    {
                        var bodyText = await reader.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(bodyText))
                        {
                            // If it's a large payload, we might want to truncate it, but let's keep it simple for now
                            requestParameter = string.IsNullOrEmpty(requestParameter) ? bodyText : $"{requestParameter} | Body: {bodyText}";
                        }
                    }
                }

                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var errorLog = new ErrorLog
                    {
                        ErrRoute = context.Request.Path.Value,
                        ErrDesc = exception.Message,
                        ErrException = exception.StackTrace?.Length > 4000 ? exception.StackTrace.Substring(0, 4000) : exception.StackTrace,
                        ErrIp = ipAddress,
                        Username = username ?? "Anonymous",
                        UserId = userId,
                        RequestParameter = requestParameter,
                        ErrDate = DateTime.UtcNow
                    };

                    dbContext.ErrorLogs.Add(errorLog);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception logEx)
            {
                // Fallback if DB logging fails
                _logger.LogError(logEx, "Failed to log exception to database.");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new 
            {
                message = "An internal system error occurred. The error has been logged.",
                error = exception.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
