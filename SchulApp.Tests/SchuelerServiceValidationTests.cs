using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class SchuelerServiceValidationTests
    {
        [Fact]
        public void Erstellen_TrimmtDenNamen()
        {
            var repository = new FakeSchuelerRepository();
            var service = new SchuelerService(repository);

            int id = service.Erstellen("  Max Muster  ", 1);

            var saved = Assert.Single(service.AlleLaden());
            Assert.Equal(id, saved.SchuelerId);
            Assert.Equal("Max Muster", saved.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Erstellen_LeererName_WirftArgumentException(string name)
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(() => service.Erstellen(name, 1));
        }

        [Fact]
        public void Erstellen_ZuLangerName_WirftArgumentException()
        {
            var service = new SchuelerService(new FakeSchuelerRepository());
            string name = new string('A', 101);

            Assert.Throws<ArgumentException>(() => service.Erstellen(name, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Erstellen_UngueltigeKlasse_WirftArgumentException(int klasseId)
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(
                () => service.Erstellen("Max Muster", klasseId)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Bearbeiten_UngueltigeId_WirftArgumentException(int id)
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(
                () => service.Bearbeiten(id, "Max Muster", 1)
            );
        }

        [Fact]
        public void Bearbeiten_NichtVorhanden_GibtFalseZurueck()
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            bool result = service.Bearbeiten(999, "Max Muster", 1);

            Assert.False(result);
        }

        [Fact]
        public void Bearbeiten_LeererName_WirftArgumentException()
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(
                () => service.Bearbeiten(1, "   ", 1)
            );
        }

        [Fact]
        public void Bearbeiten_UngueltigeKlasse_WirftArgumentException()
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(
                () => service.Bearbeiten(1, "Max Muster", 0)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Loeschen_UngueltigeId_WirftArgumentException(int id)
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            Assert.Throws<ArgumentException>(() => service.Loeschen(id));
        }

        [Fact]
        public void Loeschen_NichtVorhanden_GibtFalseZurueck()
        {
            var service = new SchuelerService(new FakeSchuelerRepository());

            bool result = service.Loeschen(999);

            Assert.False(result);
        }
    }
}
