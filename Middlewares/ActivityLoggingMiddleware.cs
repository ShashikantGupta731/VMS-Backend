using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using backend.Data;
using backend.Models.Core;
using System.Security.Claims;

namespace backend.Middlewares
{
    public class ActivityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ActivityLoggingMiddleware> _logger;

        public ActivityLoggingMiddleware(RequestDelegate next, ILogger<ActivityLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // We only log if the user is authenticated (or you can log all, but usually we want to trace the user)
            // But sometimes the authentication happens later in the pipeline, so we capture details after the request if possible,
            // or just before next.
            
            // To capture the request body (optional, might be large, only do if necessary)
            string requestBodyStr = string.Empty;
            // Uncomment if you want request body logging:
            /*
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBodyStr = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }
            */

            await _next(context);

            // After request, try to log activity
            try
            {
                // Only log API requests, skip static files or swagger
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? context.User?.FindFirst("id")?.Value;
                    
                    var username = context.User?.FindFirst(ClaimTypes.Name)?.Value 
                                ?? context.User?.FindFirst("username")?.Value;

                    // Only log if we have a known user (or you can log anonymous too)
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                        
                        using (var scope = context.RequestServices.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            var log = new UserActivityLog
                            {
                                UserId = userId,
                                Username = username,
                                IpAddress = ipAddress,
                                Method = context.Request.Method,
                                Route = context.Request.Path.Value,
                                Api = context.Request.Path.Value,
                                RequestBody = requestBodyStr.Length > 1000 ? requestBodyStr.Substring(0, 1000) : requestBodyStr,
                                CreatedOn = DateTime.UtcNow,
                                LoginStatus = "Active"
                            };

                            dbContext.UserActivityLogs.Add(log);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Do not throw here, it would crash the response which already finished
                _logger.LogError(ex, "Failed to write user activity log.");
            }
        }
    }
}
