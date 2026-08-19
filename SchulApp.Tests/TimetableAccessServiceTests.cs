using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public sealed class TimetableAccessServiceTests
    {
        private static SchulAppContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SchulAppContext>()
                .UseInMemoryDatabase($"timetable-access-{Guid.NewGuid()}")
                .Options;

            return new SchulAppContext(options);
        }

        [Fact]
        public void GetVisibleClassId_Student_ReturnsLinkedClass()
        {
            using SchulAppContext context = CreateContext();

            context.Benutzer.Add(new LoginBenutzer
            {
                Id = 1,
                Benutzername = "student",
                PasswortHash = "hash",
                Rolle = LoginBenutzer.RolleSchueler,
                SchuelerId = 10,
                ErstelltAm = DateTime.UtcNow
            });
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 10,
                Name = "Test Schüler",
                KlasseId = 7
            });
            context.SaveChanges();

            int? klasseId = TimetableAccessService.GetVisibleClassId(
                context,
                1
            );

            Assert.Equal(7, klasseId);
        }

        [Theory]
        [InlineData(LoginBenutzer.RolleAdmin)]
        [InlineData(LoginBenutzer.RolleLehrer)]
        public void GetVisibleClassId_NonStudent_ReturnsNoRestriction(
            string rolle)
        {
            using SchulAppContext context = CreateContext();

            context.Benutzer.Add(new LoginBenutzer
            {
                Id = 1,
                Benutzername = "user",
                PasswortHash = "hash",
                Rolle = rolle,
                ErstelltAm = DateTime.UtcNow
            });
            context.SaveChanges();

            int? klasseId = TimetableAccessService.GetVisibleClassId(
                context,
                1
            );

            Assert.Null(klasseId);
        }

        [Fact]
        public void GetVisibleClassId_StudentWithoutProfile_Throws()
        {
            using SchulAppContext context = CreateContext();

            context.Benutzer.Add(new LoginBenutzer
            {
                Id = 1,
                Benutzername = "student",
                PasswortHash = "hash",
                Rolle = LoginBenutzer.RolleSchueler,
                SchuelerId = null,
                ErstelltAm = DateTime.UtcNow
            });
            context.SaveChanges();

            Assert.Throws<InvalidOperationException>(() =>
                TimetableAccessService.GetVisibleClassId(context, 1)
            );
        }

        [Fact]
        public void GetVisibleClassId_StudentWithoutClass_Throws()
        {
            using SchulAppContext context = CreateContext();

            context.Benutzer.Add(new LoginBenutzer
            {
                Id = 1,
                Benutzername = "student",
                PasswortHash = "hash",
                Rolle = LoginBenutzer.RolleSchueler,
                SchuelerId = 10,
                ErstelltAm = DateTime.UtcNow
            });
            context.Schueler.Add(new SchuelerModel
            {
                SchuelerId = 10,
                Name = "Test Schüler",
                KlasseId = null
            });
            context.SaveChanges();

            Assert.Throws<InvalidOperationException>(() =>
                TimetableAccessService.GetVisibleClassId(context, 1)
            );
        }

        [Fact]
        public void GetVisibleClassId_UnknownUser_Throws()
        {
            using SchulAppContext context = CreateContext();

            Assert.Throws<UnauthorizedAccessException>(() =>
                TimetableAccessService.GetVisibleClassId(context, 999)
            );
        }
    }
}
