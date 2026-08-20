extern alias RestApi;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Xunit;
using RestAuditLogService = RestApi::schulAppREST.Services.AuditLogService;
using RestContext = RestApi::SchulApp.Data.SchulAppContext;

namespace SchulApp.Tests
{
    public class RestAuditLogServiceTests
    {
        private static RestContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RestContext>()
                .UseInMemoryDatabase($"rest-audit-tests-{Guid.NewGuid()}")
                .Options;

            return new RestContext(options);
        }

        [Fact]
        public async Task WriteAsync_OhneHttpContext_VerwendetSystem()
        {
            using var context = CreateContext();
            var service = new RestAuditLogService(
                context,
                new HttpContextAccessor()
            );

            await service.WriteAsync("CREATE", "Test", "Id=1", null);

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("System", audit.UserName);
            Assert.Equal("System", audit.UserRole);
            Assert.Equal("REST", audit.Source);
            Assert.Null(audit.Changes);
        }

        [Fact]
        public async Task WriteAsync_HeaderWerdenGetrimmtUndGespeichert()
        {
            using var context = CreateContext();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-SchulApp-User"] = "  Test User  ";
            httpContext.Request.Headers["X-SchulApp-Role"] = "  Teacher  ";
            var service = new RestAuditLogService(
                context,
                new HttpContextAccessor { HttpContext = httpContext }
            );

            await service.WriteAsync(
                "UPDATE",
                "Schueler",
                "SchuelerId=1",
                new { Name = "Max Muster" }
            );

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("Test User", audit.UserName);
            Assert.Equal("Teacher", audit.UserRole);
            Assert.Equal("UPDATE", audit.Action);
            Assert.Equal("Schueler", audit.EntityType);
            Assert.Equal("SchuelerId=1", audit.EntityId);
            Assert.NotEqual(default, audit.TimestampUtc);

            using JsonDocument changes = JsonDocument.Parse(audit.Changes!);
            Assert.Equal(
                "Max Muster",
                changes.RootElement.GetProperty("Name").GetString()
            );
        }

        [Fact]
        public async Task WriteAsync_AuthentifizierterBenutzer_HatVorrangVorHeader()
        {
            using var context = CreateContext();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-SchulApp-User"] = "Header User";
            httpContext.Request.Headers["X-SchulApp-Role"] = "Admin";
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, "Authenticated User") },
                    authenticationType: "TestAuth"
                )
            );
            var service = new RestAuditLogService(
                context,
                new HttpContextAccessor { HttpContext = httpContext }
            );

            await service.WriteAsync("DELETE", "Lehrer", "LehrerId=2", null);

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("Authenticated User", audit.UserName);
            Assert.Equal("Admin", audit.UserRole);
        }

        [Fact]
        public async Task WriteAsync_LeereHeader_FallenAufSystemZurueck()
        {
            using var context = CreateContext();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-SchulApp-User"] = "   ";
            httpContext.Request.Headers["X-SchulApp-Role"] = "   ";
            var service = new RestAuditLogService(
                context,
                new HttpContextAccessor { HttpContext = httpContext }
            );

            await service.WriteAsync("CREATE", "Kurs", "KursId=1", null);

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("System", audit.UserName);
            Assert.Equal("System", audit.UserRole);
        }
    }
}
