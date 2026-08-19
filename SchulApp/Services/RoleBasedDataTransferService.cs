using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp.Services
{
    public static class RoleBasedDataTransferService
    {
        public static async Task ExportAsync(
            int benutzerId,
            ExportDataSet dataSet,
            ExportFileFormat format,
            string filePath)
        {
            LoginBenutzer benutzer = await BenutzerLadenAsync(benutzerId);

            if (!DataTransferPermissionService.DarfExportieren(
                    benutzer.Rolle,
                    dataSet))
            {
                throw new UnauthorizedAccessException(
                    "Du hast keine Berechtigung für diesen Export."
                );
            }

            if (dataSet == ExportDataSet.Stundenplan)
            {
                int? klasseId = null;

                if (benutzer.Rolle == LoginBenutzer.RolleSchueler)
                {
                    klasseId = await EigeneKlasseLadenAsync(benutzer);
                }

                await TimetableDataTransferService.ExportAsync(
                    format,
                    filePath,
                    klasseId
                );

                return;
            }

            await DataTransferService.ExportAsync(
                dataSet,
                format,
                filePath
            );
        }

        public static async Task<CsvImportResult> ImportStudentsAsync(
            int benutzerId,
            string filePath)
        {
            LoginBenutzer benutzer = await BenutzerLadenAsync(benutzerId);

            if (!DataTransferPermissionService.DarfSchuelerImportieren(
                    benutzer.Rolle))
            {
                throw new UnauthorizedAccessException(
                    "Du hast keine Berechtigung, Schüler zu importieren."
                );
            }

            return await DataTransferService.ImportStudentsAsync(filePath);
        }

        public static async Task<CsvImportResult> ImportTimetableAsync(
            int benutzerId,
            string filePath)
        {
            LoginBenutzer benutzer = await BenutzerLadenAsync(benutzerId);

            if (!DataTransferPermissionService.DarfStundenplanImportieren(
                    benutzer.Rolle))
            {
                throw new UnauthorizedAccessException(
                    "Du hast keine Berechtigung, Stundenpläne zu importieren."
                );
            }

            return await TimetableDataTransferService.ImportAsync(filePath);
        }

        private static async Task<LoginBenutzer> BenutzerLadenAsync(
            int benutzerId)
        {
            await using SchulAppContext db = new SchulAppContext();

            LoginBenutzer? benutzer = await db.Benutzer
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == benutzerId);

            if (benutzer == null)
            {
                throw new UnauthorizedAccessException(
                    "Der angemeldete Benutzer konnte nicht gefunden werden."
                );
            }

            return benutzer;
        }

        private static async Task<int> EigeneKlasseLadenAsync(
            LoginBenutzer benutzer)
        {
            if (!benutzer.SchuelerId.HasValue)
            {
                throw new InvalidOperationException(
                    "Dein Benutzerkonto ist noch mit keinem Schüler verknüpft."
                );
            }

            await using SchulAppContext db = new SchulAppContext();

            int? klasseId = await db.Schueler
                .AsNoTracking()
                .Where(x => x.SchuelerId == benutzer.SchuelerId.Value)
                .Select(x => x.KlasseId)
                .SingleOrDefaultAsync();

            if (!klasseId.HasValue)
            {
                throw new InvalidOperationException(
                    "Für deinen verknüpften Schüler ist keine Klasse hinterlegt."
                );
            }

            return klasseId.Value;
        }
    }
}
