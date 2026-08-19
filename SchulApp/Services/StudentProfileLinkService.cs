using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp.Services
{
    public sealed class StudentProfileOption
    {
        public int SchuelerId { get; init; }

        public string Anzeige { get; init; } = string.Empty;
    }

    public static class StudentProfileLinkService
    {
        public static async Task<IReadOnlyList<StudentProfileOption>>
            VerfuegbareSchuelerLadenAsync(int benutzerId)
        {
            await using SchulAppContext db = new SchulAppContext();

            int? aktuelleSchuelerId = await db.Benutzer
                .AsNoTracking()
                .Where(x => x.Id == benutzerId)
                .Select(x => x.SchuelerId)
                .SingleOrDefaultAsync();

            HashSet<int> belegteSchuelerIds = await db.Benutzer
                .AsNoTracking()
                .Where(x => x.Id != benutzerId && x.SchuelerId != null)
                .Select(x => x.SchuelerId!.Value)
                .ToHashSetAsync();

            return await db.Schueler
                .AsNoTracking()
                .Where(x =>
                    x.SchuelerId == aktuelleSchuelerId ||
                    !belegteSchuelerIds.Contains(x.SchuelerId))
                .OrderBy(x => x.Name)
                .Select(x => new StudentProfileOption
                {
                    SchuelerId = x.SchuelerId,
                    Anzeige = x.Name
                })
                .ToListAsync();
        }

        public static async Task VerknuepfenAsync(
            int benutzerId,
            int? schuelerId)
        {
            await using SchulAppContext db = new SchulAppContext();

            LoginBenutzer? benutzer = await db.Benutzer
                .SingleOrDefaultAsync(x => x.Id == benutzerId);

            if (benutzer == null)
            {
                throw new InvalidOperationException(
                    "Das Benutzerprofil konnte nicht gefunden werden."
                );
            }

            if (schuelerId == null)
            {
                benutzer.SchuelerId = null;
                await db.SaveChangesAsync();
                return;
            }

            SchuelerModel? schueler = await db.Schueler
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.SchuelerId == schuelerId.Value);

            if (schueler == null)
            {
                throw new InvalidOperationException(
                    "Der ausgewählte Schüler existiert nicht mehr."
                );
            }

            bool bereitsVerknuepft = await db.Benutzer
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id != benutzerId &&
                    x.SchuelerId == schuelerId.Value);

            if (bereitsVerknuepft)
            {
                throw new InvalidOperationException(
                    "Dieser Schüler ist bereits mit einem anderen Benutzer verknüpft."
                );
            }

            (string? vorname, string? nachname) =
                NameAufteilen(schueler.Name);

            benutzer.SchuelerId = schueler.SchuelerId;
            benutzer.Vorname = vorname;
            benutzer.Nachname = nachname;

            await db.SaveChangesAsync();
        }

        public static bool ProfilIstVollstaendig(LoginBenutzer benutzer)
        {
            bool nameVollstaendig =
                !string.IsNullOrWhiteSpace(benutzer.Vorname) &&
                !string.IsNullOrWhiteSpace(benutzer.Nachname);

            if (benutzer.Rolle == LoginBenutzer.RolleSchueler)
            {
                return benutzer.SchuelerId.HasValue && nameVollstaendig;
            }

            return nameVollstaendig;
        }

        public static (string? Vorname, string? Nachname) NameAufteilen(
            string name)
        {
            string[] teile = name
                .Split(' ', StringSplitOptions.RemoveEmptyEntries |
                            StringSplitOptions.TrimEntries);

            if (teile.Length == 0)
            {
                return (null, null);
            }

            if (teile.Length == 1)
            {
                return (teile[0], null);
            }

            string vorname = teile[^1];
            string nachname = string.Join(" ", teile[..^1]);

            return (vorname, nachname);
        }
    }
}
