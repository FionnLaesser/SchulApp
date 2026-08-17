extern alias RestApi;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using RestContext = RestApi::SchulApp.Data.SchulAppContext;
using RestSchuelerController = RestApi::schulAppREST.Controllers.SchuelerController;
using RestSchuelerModel = RestApi::SchulApp.Models.SchuelerModel;

namespace SchulApp.Tests
{
    public class RestSchuelerControllerTests
    {
        private static RestContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RestContext>()
                .UseInMemoryDatabase($"rest-tests-{Guid.NewGuid()}")
                .Options;

            return new RestContext(options);
        }

        [Fact]
        public async Task GetSchueler_GibtAlleSchuelerZurueck()
        {
            using var context = CreateContext();
            context.Schueler.AddRange(
                new RestSchuelerModel
                {
                    SchuelerId = 1,
                    Name = "Max Muster",
                    KlasseId = 1
                },
                new RestSchuelerModel
                {
                    SchuelerId = 2,
                    Name = "Anna Beispiel",
                    KlasseId = 2
                }
            );
            await context.SaveChangesAsync();

            var controller = new RestSchuelerController(context);

            var result = await controller.GetSchueler();

            var ok = Assert.IsType<OkObjectResult>(result);
            var schueler = Assert.IsType<List<RestSchuelerModel>>(ok.Value);
            Assert.Equal(2, schueler.Count);
        }

        [Fact]
        public async Task GetSchuelerById_Vorhanden_GibtSchuelerZurueck()
        {
            using var context = CreateContext();
            context.Schueler.Add(new RestSchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();

            var controller = new RestSchuelerController(context);

            var result = await controller.GetSchuelerById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var schueler = Assert.IsType<RestSchuelerModel>(ok.Value);
            Assert.Equal(1, schueler.SchuelerId);
            Assert.Equal("Max Muster", schueler.Name);
        }

        [Fact]
        public async Task GetSchuelerById_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestSchuelerController(context);

            var result = await controller.GetSchuelerById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddSchueler_FuegtSchuelerHinzu()
        {
            using var context = CreateContext();
            var controller = new RestSchuelerController(context);
            var neuerSchueler = new RestSchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            };

            var result = await controller.AddSchueler(neuerSchueler);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RestSchuelerModel>(ok.Value);
            var gespeichert = await context.Schueler.SingleAsync();

            Assert.Equal("Max Muster", response.Name);
            Assert.Equal("Max Muster", gespeichert.Name);
            Assert.Equal(1, gespeichert.KlasseId);
        }

        [Fact]
        public async Task UpdateStudent_Vorhanden_AktualisiertSchueler()
        {
            using var context = CreateContext();
            context.Schueler.Add(new RestSchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();

            var controller = new RestSchuelerController(context);
            var geaendert = new RestSchuelerModel
            {
                SchuelerId = 999,
                Name = "Max Beispiel",
                KlasseId = 2
            };

            var result = await controller.UpdateStudent(1, geaendert);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RestSchuelerModel>(ok.Value);
            var gespeichert = await context.Schueler.SingleAsync();

            Assert.Equal(1, response.SchuelerId);
            Assert.Equal(1, gespeichert.SchuelerId);
            Assert.Equal("Max Beispiel", gespeichert.Name);
            Assert.Equal(2, gespeichert.KlasseId);
        }

        [Fact]
        public async Task UpdateStudent_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestSchuelerController(context);
            var geaendert = new RestSchuelerModel
            {
                Name = "Nicht vorhanden",
                KlasseId = 1
            };

            var result = await controller.UpdateStudent(999, geaendert);

            Assert.IsType<NotFoundResult>(result);
            Assert.Empty(context.Schueler);
        }

        [Fact]
        public async Task DelStudent_Vorhanden_LoeschtSchueler()
        {
            using var context = CreateContext();
            context.Schueler.Add(new RestSchuelerModel
            {
                SchuelerId = 1,
                Name = "Max Muster",
                KlasseId = 1
            });
            await context.SaveChangesAsync();

            var controller = new RestSchuelerController(context);

            var result = await controller.DelStudent(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(await context.Schueler.ToListAsync());
        }

        [Fact]
        public async Task DelStudent_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestSchuelerController(context);

            var result = await controller.DelStudent(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
