using System.Security.Claims;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;

namespace AccessControlAPI.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserFunctionService userFunctionService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var authorizeAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
            if (authorizeAttribute == null)
            {
                await _next(context);
                return;
            }

            var functionId = authorizeAttribute.Policy;
            if (string.IsNullOrEmpty(functionId))
            {
                await _next(context);
                return;
            }

            // Check authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Chưa đăng nhập" });
                return;
            }

            // Get userId
            var userIdClaim = context.User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Token không hợp lệ" });
                return;
            }

            // Check permission
            var userFunctions = userFunctionService.GetAllFunctionsByUserId(userId);
            if (userFunctions == null || !HasFunction(userFunctions, functionId))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = $"Không có quyền: {functionId}" });
                return;
            }

            await _next(context);
        }

        private bool HasFunction(List<DTOs.FunctionDTO> functions, string functionId)
        {
            foreach (var func in functions)
            {
                if (func.Id == functionId) return true;
                if (func.Children != null && HasFunction(func.Children, functionId)) return true;
            }
            return false;
        }
    }

    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
}