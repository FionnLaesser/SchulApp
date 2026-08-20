using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Data;
using SchulAppSOAP.Models;
using SchulAppSOAP.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class SoapSchuelerValidationTests
    {
        private static SchulAppContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SchulAppContext>()
                .UseInMemoryDatabase($"soap-validation-tests-{Guid.NewGuid()}")
                .Options;

            return new SchulAppContext(options);
        }

        private static async Task AddKlasseAsync(
            SchulAppContext context,
            int id = 1)
        {
            context.Klassen.Add(new KlasseModel
            {
                KlassenId = id,
                Bezeichnung = $"AP{id}"
            });

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddSchueler_Null_GibtFalseZurueck()
        {
            using var context = CreateContext();
            var service = new SchuelerService(context);

            bool result = await service.AddSchueler(null!);

            Assert.False(result);
            Assert.Empty(context.Schueler);
            Assert.Empty(context.AuditLogs);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddSchueler_LeererName_GibtFalseZurueck(string name)
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            var service = new SchuelerService(context);

            bool result = await service.AddSchueler(new SchuelerModel
            {
                Name = name,
                KlasseId = 1
            });

            Assert.False(result);
            Assert.Empty(context.Schueler);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task AddSchueler_UnbekannteKlasse_GibtFalseZurueck()
        {
            using var context = CreateContext();
            var service = new SchuelerService(context);

            bool result = await service.AddSchueler(new SchuelerModel
            {
                Name = "Max Muster",
                KlasseId = 999
            });

            Assert.False(result);
            Assert.Empty(context.Schueler);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task AddSchueler_BereinigtNameUndAuditIdentitaet()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            var service = new SchuelerService(context);
            var input = new SchuelerModel
            {
                Name = "  Max Muster  ",
                KlasseId = 1,
                AuditUserName = "  Test User  ",
                AuditUserRole = "  Admin  "
            };

            bool result = await service.AddSchueler(input);

            Assert.True(result);
            SchuelerModel saved = Assert.Single(context.Schueler);
            Assert.Equal("Max Muster", saved.Name);
            Assert.Null(saved.AuditUserName);
            Assert.Null(saved.AuditUserRole);

            AuditLogModel audit = Assert.Single(context.AuditLogs);
            Assert.Equal("Test User", audit.UserName);
            Assert.Equal("Admin", audit.UserRole);
            Assert.Equal("SOAP", audit.Source);
        }

        [Fact]
        public async Task UpdateSchueler_UngueltigeId_GibtFalseZurueck()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            var service = new SchuelerService(context);

            bool result = await service.UpdateSchueler(new SchuelerModel
            {
                SchuelerId = 0,
                Name = "Max Muster",
                KlasseId = 1
            });

            Assert.False(result);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task UpdateSchueler_LeererName_GibtFalseZurueck()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();
            var service = new SchuelerService(context);

            bool result = await service.UpdateSchueler(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "   ",
                KlasseId = 1
            });

            Assert.False(result);
            Assert.Equal("Max Muster", (await context.Schueler.FindAsync(1))!.Name);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task UpdateSchueler_NichtVorhanden_GibtFalseZurueck()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            var service = new SchuelerService(context);

            bool result = await service.UpdateSchueler(new SchuelerModel
            {
                SchuelerId = 999,
                Name = "Max Muster",
                KlasseId = 1
            });

            Assert.False(result);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task UpdateSchueler_UnbekannteKlasse_AendertKeineDaten()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();
            var service = new SchuelerService(context);

            bool result = await service.UpdateSchueler(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Beispiel",
                KlasseId = 999
            });

            Assert.False(result);
            SchuelerModel saved = (await context.Schueler.FindAsync(1))!;
            Assert.Equal("Max Muster", saved.Name);
            Assert.Equal(1, saved.KlasseId);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task UpdateSchueler_OhneAenderung_ErstelltKeinenAuditLog()
        {
            using var context = CreateContext();
            await AddKlasseAsync(context);
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();
            var service = new SchuelerService(context);

            bool result = await service.UpdateSchueler(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });

            Assert.True(result);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task DeleteSchuelerMitAudit_VerwendetAngegebenenBenutzer()
        {
            using var context = CreateContext();
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();
            var service = new SchuelerService(context);

            bool result = await service.DeleteSchuelerMitAudit(
                1,
                "  Test User  ",
                "  Admin  "
            );

            Assert.True(result);
            Assert.Empty(context.Schueler);
            AuditLogModel audit = Assert.Single(context.AuditLogs);
            Assert.Equal("Test User", audit.UserName);
            Assert.Equal("Admin", audit.UserRole);
            Assert.Equal("DELETE", audit.Action);
            Assert.Equal("SOAP", audit.Source);
        }
    }
}
