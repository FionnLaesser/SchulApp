using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class StudentImportTemplateServiceTests
    {
        [Fact]
        public void CreateTemplate_UsesRequiredImportHeadersAndExampleRow()
        {
            string template = StudentImportTemplateService.CreateTemplate();

            Assert.StartsWith("Name;KlasseId", template);
            Assert.Contains("Müller Anna;1", template);
        }

        [Fact]
        public void CreateTemplate_CanBeValidatedByStudentImportParser()
        {
            string template = StudentImportTemplateService.CreateTemplate();

            StudentCsvValidationResult result =
                DataTransferService.ValidateStudentCsv(
                    template,
                    new[] { 1 }
                );

            Assert.Empty(result.Errors);
            Assert.Single(result.Students);
            Assert.Equal("Müller Anna", result.Students[0].Name);
            Assert.Equal(1, result.Students[0].KlasseId);
        }
    }
}
