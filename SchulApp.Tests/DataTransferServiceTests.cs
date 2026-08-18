using SchulApp.Models;
using SchulApp.Services;
using System.Text;

namespace SchulApp.Tests
{
    public class DataTransferServiceTests
    {
        [Fact]
        public void CreateCsv_EscapesQuotesAndKeepsUmlauts()
        {
            IReadOnlyList<string> headers = new[]
            {
                "Name",
                "Klasse"
            };

            IReadOnlyList<IReadOnlyList<string>> rows = new[]
            {
                (IReadOnlyList<string>)new[]
                {
                    "Müller \"Max\"",
                    "AP23a"
                }
            };

            string csv = DataTransferService.CreateCsv(
                headers,
                rows
            );

            Assert.Contains(
                "\"Müller \"\"Max\"\"\";AP23a",
                csv
            );
        }

        [Fact]
        public void ValidateStudentCsv_AcceptsAtLeastFiftyValidStudents()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Name;KlasseId");

            for (int i = 1; i <= 50; i++)
            {
                csv.AppendLine($"Schüler {i};1");
            }

            StudentCsvValidationResult result =
                DataTransferService.ValidateStudentCsv(
                    csv.ToString(),
                    new[] { 1 }
                );

            Assert.Empty(result.Errors);
            Assert.Equal(50, result.Students.Count);
            Assert.All(
                result.Students,
                student => Assert.Equal(1, student.KlasseId)
            );
        }

        [Fact]
        public void ValidateStudentCsv_ReportsMissingInvalidAndDuplicateRows()
        {
            string csv =
                "Name;KlasseId\n" +
                "Alice;1\n" +
                ";1\n" +
                "Bob;999\n" +
                "Alice;1\n";

            StudentCsvValidationResult result =
                DataTransferService.ValidateStudentCsv(
                    csv,
                    new[] { 1 }
                );

            Assert.Single(result.Students);
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains(
                result.Errors,
                error => error.Contains("Name fehlt")
            );
            Assert.Contains(
                result.Errors,
                error => error.Contains("existiert nicht")
            );
            Assert.Contains(
                result.Errors,
                error => error.Contains("bereits vorhanden")
            );
        }

        [Fact]
        public void ValidateStudentCsv_DetectsExistingStudentInSameClass()
        {
            string csv =
                "Name,KlasseId\n" +
                "Müller,2\n";

            SchuelerModel existingStudent = new SchuelerModel
            {
                Name = "müller",
                KlasseId = 2
            };

            StudentCsvValidationResult result =
                DataTransferService.ValidateStudentCsv(
                    csv,
                    new[] { 2 },
                    new[] { existingStudent }
                );

            Assert.Empty(result.Students);
            Assert.Single(result.Errors);
            Assert.Contains(
                "bereits vorhanden",
                result.Errors[0]
            );
        }

        [Fact]
        public void ValidateStudentCsv_AllowsQuotedDelimiterInName()
        {
            string csv =
                "Name;KlasseId\n" +
                "\"Müller; Max\";3\n";

            StudentCsvValidationResult result =
                DataTransferService.ValidateStudentCsv(
                    csv,
                    new[] { 3 }
                );

            Assert.Empty(result.Errors);
            SchuelerModel student = Assert.Single(result.Students);
            Assert.Equal("Müller; Max", student.Name);
        }

        [Fact]
        public void CreatePdf_CreatesValidLookingPdfWithUmlauts()
        {
            IReadOnlyList<string> headers = new[]
            {
                "Name",
                "Klasse"
            };

            IReadOnlyList<IReadOnlyList<string>> rows = new[]
            {
                (IReadOnlyList<string>)new[]
                {
                    "Müller",
                    "AP23a"
                }
            };

            byte[] pdf = DataTransferService.CreatePdf(
                "Schülerliste",
                headers,
                rows
            );

            string pdfText = Encoding.Latin1.GetString(pdf);

            Assert.StartsWith("%PDF-1.4", pdfText);
            Assert.Contains("Schülerliste", pdfText);
            Assert.Contains("Müller", pdfText);
            Assert.Contains("%%EOF", pdfText);
            Assert.True(pdf.Length > 500);
        }
    }
}
