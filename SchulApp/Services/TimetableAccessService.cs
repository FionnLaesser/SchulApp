using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp.Services
{
    public static class TimetableAccessService
    {
        public static int? GetVisibleClassId(int benutzerId)
        {
            using SchulAppContext db = new SchulAppContext();
            return GetVisibleClassId(db, benutzerId);
        }

        public static int? GetVisibleClassId(
            SchulAppContext db,
            int benutzerId)
        {
            ArgumentNullException.ThrowIfNull(db);

            if (benutzerId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Der angemeldete Benutzer ist ungültig."
                );
            }

            var benutzer = db.Benutzer
                .AsNoTracking()
                .Where(x => x.Id == benutzerId)
                .Select(x => new
                {
                    x.Rolle,
                    x.SchuelerId
                })
                .SingleOrDefault();

            if (benutzer == null)
            {
                throw new UnauthorizedAccessException(
                    "Der angemeldete Benutzer konnte nicht gefunden werden."
                );
            }

            if (benutzer.Rolle != LoginBenutzer.RolleSchueler)
            {
                return null;
            }

            if (!benutzer.SchuelerId.HasValue)
            {
                throw new InvalidOperationException(
                    "Dein Benutzerkonto ist noch mit keinem Schüler verknüpft."
                );
            }

            int? klasseId = db.Schueler
                .AsNoTracking()
                .Where(x => x.SchuelerId == benutzer.SchuelerId.Value)
                .Select(x => x.KlasseId)
                .SingleOrDefault();

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
