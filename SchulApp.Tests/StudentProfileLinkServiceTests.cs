using SchulApp.Models;
using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class StudentProfileLinkServiceTests
    {
        [Fact]
        public void NameAufteilen_TeiltBestehendesSchuelerNamensformat()
        {
            (string? vorname, string? nachname) =
                StudentProfileLinkService.NameAufteilen("Müller Anna");

            Assert.Equal("Anna", vorname);
            Assert.Equal("Müller", nachname);
        }

        [Fact]
        public void ProfilIstVollstaendig_SchuelerBrauchtVerknuepfung()
        {
            LoginBenutzer benutzer = new LoginBenutzer
            {
                Rolle = LoginBenutzer.RolleSchueler,
                Vorname = "Anna",
                Nachname = "Müller",
                SchuelerId = null
            };

            Assert.False(
                StudentProfileLinkService.ProfilIstVollstaendig(benutzer)
            );
        }

        [Fact]
        public void ProfilIstVollstaendig_VerknuepfterSchuelerIstVollstaendig()
        {
            LoginBenutzer benutzer = new LoginBenutzer
            {
                Rolle = LoginBenutzer.RolleSchueler,
                Vorname = "Anna",
                Nachname = "Müller",
                SchuelerId = 7
            };

            Assert.True(
                StudentProfileLinkService.ProfilIstVollstaendig(benutzer)
            );
        }
    }
}
