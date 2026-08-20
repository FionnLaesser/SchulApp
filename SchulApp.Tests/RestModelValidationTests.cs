extern alias RestApi;

using System.ComponentModel.DataAnnotations;
using Xunit;
using RestKlasseModel = RestApi::SchulApp.Models.KlasseModel;
using RestKursModel = RestApi::SchulApp.Models.KursModel;
using RestLehrerModel = RestApi::SchulApp.Models.LehrerModel;

namespace SchulApp.Tests
{
    public class RestModelValidationTests
    {
        private static List<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                results,
                validateAllProperties: true
            );

            return results;
        }

        [Fact]
        public void Lehrer_LeererName_IstUngueltig()
        {
            var model = new RestLehrerModel
            {
                Name = string.Empty
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestLehrerModel.Name))
            );
        }

        [Fact]
        public void Lehrer_ZuLangerName_IstUngueltig()
        {
            var model = new RestLehrerModel
            {
                Name = new string('A', 101)
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestLehrerModel.Name))
            );
        }

        [Fact]
        public void Lehrer_ZuLangeKontaktwerte_SindUngueltig()
        {
            var model = new RestLehrerModel
            {
                Name = "Test Lehrer",
                Email = new string('e', 256),
                Telefon = new string('1', 51)
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestLehrerModel.Email))
            );
            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestLehrerModel.Telefon))
            );
        }

        [Fact]
        public void Klasse_LeereBezeichnung_IstUngueltig()
        {
            var model = new RestKlasseModel
            {
                Bezeichnung = string.Empty
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestKlasseModel.Bezeichnung))
            );
        }

        [Fact]
        public void Klasse_ZuLangeBezeichnung_IstUngueltig()
        {
            var model = new RestKlasseModel
            {
                Bezeichnung = new string('A', 51)
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestKlasseModel.Bezeichnung))
            );
        }

        [Fact]
        public void Kurs_LeererName_IstUngueltig()
        {
            var model = new RestKursModel
            {
                Name = string.Empty,
                KlasseId = 1,
                LehrerId = 1
            };

            List<ValidationResult> results = Validate(model);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(nameof(RestKursModel.Name))
            );
        }

        [Fact]
        public void GueltigeRestModelle_HabenKeineValidierungsfehler()
        {
            var lehrer = new RestLehrerModel
            {
                Name = "Test Lehrer",
                Email = "test@example.com",
                Telefon = "+41 79 000 00 00"
            };
            var klasse = new RestKlasseModel
            {
                Bezeichnung = "AP23a",
                KlassenlehrerId = 1
            };
            var kurs = new RestKursModel
            {
                Name = "Mathematik",
                KlasseId = 1,
                LehrerId = 1
            };

            Assert.Empty(Validate(lehrer));
            Assert.Empty(Validate(klasse));
            Assert.Empty(Validate(kurs));
        }
    }
}
