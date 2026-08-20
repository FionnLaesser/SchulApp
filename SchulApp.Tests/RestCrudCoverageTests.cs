extern alias RestApi;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Xunit;
using RestAuditLogService = RestApi::schulAppREST.Services.AuditLogService;
using RestContext = RestApi::SchulApp.Data.SchulAppContext;
using RestKlasseController = RestApi::schulAppREST.Controllers.KlasseController;
using RestKlasseModel = RestApi::SchulApp.Models.KlasseModel;
using RestKursController = RestApi::schulAppREST.Controllers.KursController;
using RestKursModel = RestApi::SchulApp.Models.KursModel;
using RestLehrerController = RestApi::schulAppREST.Controllers.LehrerController;
using RestLehrerModel = RestApi::SchulApp.Models.LehrerModel;

namespace SchulApp.Tests
{
    public class RestCrudCoverageTests
    {
        private static RestContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RestContext>()
                .UseInMemoryDatabase($"rest-crud-tests-{Guid.NewGuid()}")
                .Options;

            return new RestContext(options);
        }

        private static RestAuditLogService CreateAuditLogService(
            RestContext context,
            string user = "Coverage User",
            string role = "Admin")
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-SchulApp-User"] = user;
            httpContext.Request.Headers["X-SchulApp-Role"] = role;

            return new RestAuditLogService(
                context,
                new HttpContextAccessor { HttpContext = httpContext }
            );
        }

        [Fact]
        public async Task Klasse_Get_GibtAlleKlassenZurueck()
        {
            using var context = CreateContext();
            context.Klasse.AddRange(
                new RestKlasseModel { KlassenId = 1, Bezeichnung = "AP23a" },
                new RestKlasseModel { KlassenId = 2, Bezeichnung = "AP23b" }
            );
            await context.SaveChangesAsync();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.GetKlasse();

            var ok = Assert.IsType<OkObjectResult>(result);
            var values = Assert.IsType<List<RestKlasseModel>>(ok.Value);
            Assert.Equal(2, values.Count);
        }

        [Fact]
        public async Task Klasse_GetById_Vorhanden_GibtKlasseZurueck()
        {
            using var context = CreateContext();
            context.Klasse.Add(new RestKlasseModel
            {
                KlassenId = 1,
                Bezeichnung = "AP23a"
            });
            await context.SaveChangesAsync();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.GetKlasseById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<RestKlasseModel>(ok.Value);
            Assert.Equal("AP23a", value.Bezeichnung);
        }

        [Fact]
        public async Task Klasse_GetById_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.GetKlasseById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Klasse_Add_SpeichertKlasse()
        {
            using var context = CreateContext();
            var controller = new RestKlasseController(context);
            var input = new RestKlasseModel
            {
                KlassenId = 1,
                Bezeichnung = "AP23a",
                KlassenlehrerId = 7
            };

            IActionResult result = await controller.AddKlasse(input);

            Assert.IsType<OkObjectResult>(result);
            RestKlasseModel saved = await context.Klasse.SingleAsync();
            Assert.Equal("AP23a", saved.Bezeichnung);
            Assert.Equal(7, saved.KlassenlehrerId);
        }

        [Fact]
        public async Task Klasse_Update_AktualisiertWerteUndBehaeltId()
        {
            using var context = CreateContext();
            context.Klasse.Add(new RestKlasseModel
            {
                KlassenId = 1,
                Bezeichnung = "AP23a",
                KlassenlehrerId = 7
            });
            await context.SaveChangesAsync();
            var controller = new RestKlasseController(context);
            var input = new RestKlasseModel
            {
                KlassenId = 999,
                Bezeichnung = "AP24a",
                KlassenlehrerId = 8
            };

            IActionResult result = await controller.UpdateTeacher(1, input);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RestKlasseModel>(ok.Value);
            Assert.Equal(1, response.KlassenId);
            Assert.Equal("AP24a", response.Bezeichnung);
            Assert.Equal(8, response.KlassenlehrerId);
        }

        [Fact]
        public async Task Klasse_Update_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.UpdateTeacher(
                999,
                new RestKlasseModel { Bezeichnung = "AP24a" }
            );

            Assert.IsType<NotFoundResult>(result);
            Assert.Empty(context.Klasse);
        }

        [Fact]
        public async Task Klasse_Delete_Vorhanden_LoeschtKlasse()
        {
            using var context = CreateContext();
            context.Klasse.Add(new RestKlasseModel
            {
                KlassenId = 1,
                Bezeichnung = "AP23a"
            });
            await context.SaveChangesAsync();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.DelTeacher(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(context.Klasse);
        }

        [Fact]
        public async Task Klasse_Delete_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKlasseController(context);

            IActionResult result = await controller.DelTeacher(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Kurs_Get_GibtAlleKurseZurueck()
        {
            using var context = CreateContext();
            context.Kurs.AddRange(
                new RestKursModel
                {
                    KursId = 1,
                    Name = "Mathematik",
                    KlasseId = 1,
                    LehrerId = 1
                },
                new RestKursModel
                {
                    KursId = 2,
                    Name = "Deutsch",
                    KlasseId = 1,
                    LehrerId = 2
                }
            );
            await context.SaveChangesAsync();
            var controller = new RestKursController(context);

            IActionResult result = await controller.GetKurs();

            var ok = Assert.IsType<OkObjectResult>(result);
            var values = Assert.IsType<List<RestKursModel>>(ok.Value);
            Assert.Equal(2, values.Count);
        }

        [Fact]
        public async Task Kurs_GetById_Vorhanden_GibtKursZurueck()
        {
            using var context = CreateContext();
            context.Kurs.Add(new RestKursModel
            {
                KursId = 1,
                Name = "Mathematik",
                KlasseId = 1,
                LehrerId = 1
            });
            await context.SaveChangesAsync();
            var controller = new RestKursController(context);

            IActionResult result = await controller.GetKursById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<RestKursModel>(ok.Value);
            Assert.Equal("Mathematik", value.Name);
        }

        [Fact]
        public async Task Kurs_GetById_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKursController(context);

            IActionResult result = await controller.GetKursById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Kurs_Add_SpeichertKurs()
        {
            using var context = CreateContext();
            var controller = new RestKursController(context);
            var input = new RestKursModel
            {
                KursId = 1,
                Name = "Mathematik",
                KlasseId = 2,
                LehrerId = 3
            };

            IActionResult result = await controller.AddKurs(input);

            Assert.IsType<OkObjectResult>(result);
            RestKursModel saved = await context.Kurs.SingleAsync();
            Assert.Equal("Mathematik", saved.Name);
            Assert.Equal(2, saved.KlasseId);
            Assert.Equal(3, saved.LehrerId);
        }

        [Fact]
        public async Task Kurs_Update_AktualisiertWerteUndBehaeltId()
        {
            using var context = CreateContext();
            context.Kurs.Add(new RestKursModel
            {
                KursId = 1,
                Name = "Mathematik",
                KlasseId = 1,
                LehrerId = 1
            });
            await context.SaveChangesAsync();
            var controller = new RestKursController(context);
            var input = new RestKursModel
            {
                KursId = 999,
                Name = "Deutsch",
                KlasseId = 2,
                LehrerId = 3
            };

            IActionResult result = await controller.UpdateKurs(1, input);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RestKursModel>(ok.Value);
            Assert.Equal(1, response.KursId);
            Assert.Equal("Deutsch", response.Name);
            Assert.Equal(2, response.KlasseId);
            Assert.Equal(3, response.LehrerId);
        }

        [Fact]
        public async Task Kurs_Update_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKursController(context);

            IActionResult result = await controller.UpdateKurs(
                999,
                new RestKursModel
                {
                    Name = "Deutsch",
                    KlasseId = 1,
                    LehrerId = 1
                }
            );

            Assert.IsType<NotFoundResult>(result);
            Assert.Empty(context.Kurs);
        }

        [Fact]
        public async Task Kurs_Delete_Vorhanden_LoeschtKurs()
        {
            using var context = CreateContext();
            context.Kurs.Add(new RestKursModel
            {
                KursId = 1,
                Name = "Mathematik",
                KlasseId = 1,
                LehrerId = 1
            });
            await context.SaveChangesAsync();
            var controller = new RestKursController(context);

            IActionResult result = await controller.DelKurs(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(context.Kurs);
        }

        [Fact]
        public async Task Kurs_Delete_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestKursController(context);

            IActionResult result = await controller.DelKurs(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Lehrer_Get_GibtAlleLehrerZurueck()
        {
            using var context = CreateContext();
            context.Lehrer.AddRange(
                new RestLehrerModel { LehrerId = 1, Name = "Lehrer Eins" },
                new RestLehrerModel { LehrerId = 2, Name = "Lehrer Zwei" }
            );
            await context.SaveChangesAsync();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.GetLehrer();

            var ok = Assert.IsType<OkObjectResult>(result);
            var values = Assert.IsType<List<RestLehrerModel>>(ok.Value);
            Assert.Equal(2, values.Count);
        }

        [Fact]
        public async Task Lehrer_GetById_Vorhanden_GibtLehrerZurueck()
        {
            using var context = CreateContext();
            context.Lehrer.Add(new RestLehrerModel
            {
                LehrerId = 1,
                Name = "Lehrer Eins"
            });
            await context.SaveChangesAsync();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.GetLehrerById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<RestLehrerModel>(ok.Value);
            Assert.Equal("Lehrer Eins", value.Name);
        }

        [Fact]
        public async Task Lehrer_GetById_NichtVorhanden_GibtNotFoundZurueck()
        {
            using var context = CreateContext();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.GetLehrerById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Lehrer_Add_SpeichertLehrerUndAuditLog()
        {
            using var context = CreateContext();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );
            var input = new RestLehrerModel
            {
                LehrerId = 1,
                Name = "Lehrer Eins",
                Email = "eins@example.com",
                Telefon = "+41 79 000 00 01"
            };

            IActionResult result = await controller.AddLehrer(input);

            Assert.IsType<OkObjectResult>(result);
            RestLehrerModel saved = await context.Lehrer.SingleAsync();
            Assert.Equal("Lehrer Eins", saved.Name);

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("CREATE", audit.Action);
            Assert.Equal("Lehrer", audit.EntityType);
            Assert.Equal("REST", audit.Source);
            Assert.Equal("Coverage User", audit.UserName);
            Assert.Equal("Admin", audit.UserRole);
        }

        [Fact]
        public async Task Lehrer_Update_AktualisiertWerteUndAuditLog()
        {
            using var context = CreateContext();
            context.Lehrer.Add(new RestLehrerModel
            {
                LehrerId = 1,
                Name = "Lehrer Alt",
                Email = "alt@example.com",
                Telefon = "111"
            });
            await context.SaveChangesAsync();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );
            var input = new RestLehrerModel
            {
                LehrerId = 999,
                Name = "Lehrer Neu",
                Email = "neu@example.com",
                Telefon = "222"
            };

            IActionResult result = await controller.UpdateTeacher(1, input);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RestLehrerModel>(ok.Value);
            Assert.Equal(1, response.LehrerId);
            Assert.Equal("Lehrer Neu", response.Name);

            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("UPDATE", audit.Action);
            Assert.Equal("LehrerId=1", audit.EntityId);
            using JsonDocument changes = JsonDocument.Parse(audit.Changes!);
            Assert.Equal(
                "Lehrer Alt",
                changes.RootElement.GetProperty("Name").GetProperty("Old").GetString()
            );
            Assert.Equal(
                "Lehrer Neu",
                changes.RootElement.GetProperty("Name").GetProperty("New").GetString()
            );
        }

        [Fact]
        public async Task Lehrer_Update_NichtVorhanden_GibtNotFoundOhneAuditLogZurueck()
        {
            using var context = CreateContext();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.UpdateTeacher(
                999,
                new RestLehrerModel { Name = "Nicht vorhanden" }
            );

            Assert.IsType<NotFoundResult>(result);
            Assert.Empty(context.Lehrer);
            Assert.Empty(context.AuditLogs);
        }

        [Fact]
        public async Task Lehrer_Delete_Vorhanden_LoeschtLehrerUndSchreibtAuditLog()
        {
            using var context = CreateContext();
            context.Lehrer.Add(new RestLehrerModel
            {
                LehrerId = 1,
                Name = "Lehrer Eins",
                Email = "eins@example.com",
                Telefon = "111"
            });
            await context.SaveChangesAsync();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.DelTeacher(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(context.Lehrer);
            var audit = Assert.Single(context.AuditLogs);
            Assert.Equal("DELETE", audit.Action);
            Assert.Equal("LehrerId=1", audit.EntityId);
            Assert.Equal("REST", audit.Source);
        }

        [Fact]
        public async Task Lehrer_Delete_NichtVorhanden_GibtNotFoundOhneAuditLogZurueck()
        {
            using var context = CreateContext();
            var controller = new RestLehrerController(
                context,
                CreateAuditLogService(context)
            );

            IActionResult result = await controller.DelTeacher(999);

            Assert.IsType<NotFoundResult>(result);
            Assert.Empty(context.AuditLogs);
        }
    }
}
