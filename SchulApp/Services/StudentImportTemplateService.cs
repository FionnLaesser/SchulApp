using System.Text;

namespace SchulApp.Services
{
    public static class StudentImportTemplateService
    {
        public static string CreateTemplate()
        {
            IReadOnlyList<string> headers = new[]
            {
                "Name",
                "KlasseId"
            };

            IReadOnlyList<IReadOnlyList<string>> rows = new[]
            {
                (IReadOnlyList<string>)new[]
                {
                    "Müller Anna",
                    "1"
                }
            };

            return DataTransferService.CreateCsv(headers, rows);
        }

        public static Task SaveTemplateAsync(string filePath)
        {
            return File.WriteAllTextAsync(
                filePath,
                CreateTemplate(),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)
            );
        }
    }
}
