using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class SchuelerServiceTests
    {
        [Fact]
        public void Erstellen_FuegtSchuelerHinzu()
        {
            FakeSchuelerRepository repository = new FakeSchuelerRepository();
            SchuelerService service = new SchuelerService(repository);

            int id = service.Erstellen("Max Muster", 1);

            var schueler = service.AlleLaden();
            Assert.Single(schueler);
            Assert.Equal(id, schueler[0].SchuelerId);
            Assert.Equal("Max Muster", schueler[0].Name);
            Assert.Equal(1, schueler[0].KlasseId);
        }

        [Fact]
        public void Bearbeiten_AendertNameUndKlasse()
        {
            FakeSchuelerRepository repository = new FakeSchuelerRepository();
            SchuelerService service = new SchuelerService(repository);
            int id = service.Erstellen("Max Muster", 1);

            bool bearbeitet = service.Bearbeiten(id, "Max Beispiel", 2);

            var schueler = service.AlleLaden();
            Assert.True(bearbeitet);
            Assert.Single(schueler);
            Assert.Equal("Max Beispiel", schueler[0].Name);
            Assert.Equal(2, schueler[0].KlasseId);
        }

        [Fact]
        public void Loeschen_EntferntSchueler()
        {
            FakeSchuelerRepository repository = new FakeSchuelerRepository();
            SchuelerService service = new SchuelerService(repository);
            int id = service.Erstellen("Max Muster", 1);

            bool geloescht = service.Loeschen(id);

            Assert.True(geloescht);
            Assert.Empty(service.AlleLaden());
        }
    }
}
