using SchulApp.Data;
using SchulApp.Models;
using System.Text.Json;

namespace schulAppREST.Services
{
    public sealed class AuditLogService
    {
        private readonly SchulAppContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            SchulAppContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task WriteAsync(
            string action,
            string entityType,
            string? entityId,
            object? changes)
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            string? suppliedUser = httpContext?.Request.Headers["X-SchulApp-User"].FirstOrDefault();
            string? suppliedRole = httpContext?.Request.Headers["X-SchulApp-Role"].FirstOrDefault();
            string? authenticatedUser = httpContext?.User?.Identity?.IsAuthenticated == true
                ? httpContext.User.Identity.Name
                : null;

            string userName = FirstValue(authenticatedUser, suppliedUser, "System");
            string userRole = FirstValue(suppliedRole, "System");

            _context.AuditLogs.Add(new AuditLogModel
            {
                TimestampUtc = DateTime.UtcNow,
                UserName = userName,
                UserRole = userRole,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Changes = changes == null ? null : JsonSerializer.Serialize(changes),
                Source = "REST"
            });

            await _context.SaveChangesAsync();
        }

        private static string FirstValue(params string?[] values)
        {
            return values.First(x => !string.IsNullOrWhiteSpace(x))!.Trim();
        }
    }
}
