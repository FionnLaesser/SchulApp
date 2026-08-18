using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Data;
using SchulAppSOAP.Models;
using SchulAppSOAP.Services;
using System.Text.Json;
using Xunit;

namespace SchulApp.Tests
{
    public class SoapSchuelerServiceTests
    {
        private static SchulAppContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SchulAppContext>()
                .UseInMemoryDatabase($"soap-tests-{Guid.NewGuid()}")
                .Options;

            return new SchulAppContext(options);
        }

        [Fact]
        public async Task GetSchueler_GibtAlleSchuelerZurueck()
        {
            using var context = CreateContext();
            context.Schueler.AddRange(
                new SchuelerModel
                {
                    SchuelerId = 1,
                    Name = "Max Muster",
                    KlasseId = 1
                },
                new SchuelerModel
                {
                    SchuelerId = 2,
                    Name = "Anna Beispiel",
                    KlasseId = 2
                }
            );
            await context.SaveChangesAsync();

            var service = new SchuelerService(context);

            var schueler = await service.GetSchueler();

            Assert.Equal(2, schueler.Count);
            Assert.Contains(schueler, s => s.Name == "Max Muster");
            Assert.Contains(schueler, s => s.Name == "Anna Beispiel");
        }

        [Fact]
        public async Task GetSchuelerById_Vorhanden_GibtSchuelerZurueck()
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

            var schueler = await service.GetSchuelerById(1);

            Assert.NotNull(schueler);
            Assert.Equal(1, schueler.SchuelerId);
            Assert.Equal("Max Muster", schueler.Name);
        }

        [Fact]
        public async Task GetSchuelerById_NichtVorhanden_GibtNullZurueck()
        {
            using var context = CreateContext();
            var service = new SchuelerService(context);

            var schueler = await service.GetSchuelerById(999);

            Assert.Null(schueler);
        }

        [Fact]
        public async Task AddSchueler_Erfolgreich_ErstelltAuditLog()
        {
            using var context = CreateContext();
            context.Klassen.Add(new KlasseModel
            {
                KlassenId = 1,
                Bezeichnung = "AP23a"
            });
            await context.SaveChangesAsync();

            var service = new SchuelerService(context);

            bool erstellt = await service.AddSchueler(
                new SchuelerModel
                {
                    Name = "Max Muster",
                    KlasseId = 1
                }
            );

            Assert.True(erstellt);

            AuditLogModel auditLog = Assert.Single(context.AuditLogs);
            Assert.Equal("CREATE", auditLog.Action);
            Assert.Equal("Schueler", auditLog.EntityType);
            Assert.Equal("SOAP", auditLog.Source);
            Assert.Equal("System", auditLog.UserName);
            Assert.StartsWith("SchuelerId=", auditLog.EntityId);
            Assert.NotEqual(default, auditLog.TimestampUtc);

            using JsonDocument changes = JsonDocument.Parse(auditLog.Changes!);
            Assert.Equal(
                "Max Muster",
                changes.RootElement.GetProperty("Name").GetString()
            );
            Assert.Equal(
                1,
                changes.RootElement.GetProperty("KlasseId").GetInt32()
            );
        }

        [Fact]
        public async Task UpdateSchueler_Erfolgreich_ErstelltAuditLogMitAenderungen()
        {
            using var context = CreateContext();
            context.Klassen.AddRange(
                new KlasseModel
                {
                    KlassenId = 1,
                    Bezeichnung = "AP23a"
                },
                new KlasseModel
                {
                    KlassenId = 2,
                    Bezeichnung = "AP23b"
                }
            );
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();

            var service = new SchuelerService(context);

            bool bearbeitet = await service.UpdateSchueler(
                new SchuelerModel
                {
                    SchuelerId = 1,
                    Name = "Max Beispiel",
                    KlasseId = 2
                }
            );

            Assert.True(bearbeitet);

            AuditLogModel auditLog = Assert.Single(context.AuditLogs);
            Assert.Equal("UPDATE", auditLog.Action);
            Assert.Equal("Schueler", auditLog.EntityType);
            Assert.Equal("SchuelerId=1", auditLog.EntityId);
            Assert.Equal("SOAP", auditLog.Source);

            using JsonDocument changes = JsonDocument.Parse(auditLog.Changes!);

            JsonElement name = changes.RootElement.GetProperty("Name");
            Assert.Equal("Max Muster", name.GetProperty("Old").GetString());
            Assert.Equal("Max Beispiel", name.GetProperty("New").GetString());

            JsonElement klasseId = changes.RootElement.GetProperty("KlasseId");
            Assert.Equal(1, klasseId.GetProperty("Old").GetInt32());
            Assert.Equal(2, klasseId.GetProperty("New").GetInt32());
        }

        [Fact]
        public async Task DeleteSchueler_Vorhanden_LoeschtSchueler()
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

            var geloescht = await service.DeleteSchueler(1);

            Assert.True(geloescht);
            Assert.Empty(await context.Schueler.ToListAsync());

            AuditLogModel auditLog = Assert.Single(context.AuditLogs);
            Assert.Equal("DELETE", auditLog.Action);
            Assert.Equal("Schueler", auditLog.EntityType);
            Assert.Equal("SchuelerId=1", auditLog.EntityId);
            Assert.Equal("SOAP", auditLog.Source);

            using JsonDocument changes = JsonDocument.Parse(auditLog.Changes!);
            Assert.Equal(
                "Max Muster",
                changes.RootElement.GetProperty("Name").GetString()
            );
            Assert.Equal(
                1,
                changes.RootElement.GetProperty("KlasseId").GetInt32()
            );
        }

        [Fact]
        public async Task DeleteSchueler_NichtVorhanden_GibtFalseZurueck()
        {
            using var context = CreateContext();
            var service = new SchuelerService(context);

            var geloescht = await service.DeleteSchueler(999);

            Assert.False(geloescht);
            Assert.Empty(context.Schueler);
            Assert.Empty(context.AuditLogs);
        }
    }
}
