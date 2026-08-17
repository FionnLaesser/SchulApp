using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Data;
using SchulAppSOAP.Models;
using SchulAppSOAP.Services;
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
        }

        [Fact]
        public async Task DeleteSchueler_NichtVorhanden_GibtFalseZurueck()
        {
            using var context = CreateContext();
            var service = new SchuelerService(context);

            var geloescht = await service.DeleteSchueler(999);

            Assert.False(geloescht);
            Assert.Empty(context.Schueler);
        }
    }
}
