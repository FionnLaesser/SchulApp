using Microsoft.EntityFrameworkCore;
using SchulApp.Models;

namespace SchulApp.Data
{
    public class SqlSchuelerRepository : ISchuelerRepository
    {
        public IReadOnlyList<SchuelerModel> AlleLaden()
        {
            // Erstellt den Entity-Framework-Datenbankkontext
            using SchulAppContext context = new SchulAppContext();

            // Lädt alle Schüler inklusive ihrer zugehörigen Klasse.
            // Entity Framework erstellt SELECT und JOIN automatisch.
            List<SchuelerModel> ergebnis = context.Schueler
                .Include(s => s.Klasse)
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToList();

            return ergebnis;
        }

        public int Erstellen(string name, int klasseId)
        {
            using SchulAppContext context = new SchulAppContext();

            // Erstellt ein neues Schüler-Objekt
            SchuelerModel neuerSchueler = new SchuelerModel
            {
                Name = name,
                KlasseId = klasseId
            };

            // Fügt den Schüler zu Entity Framework hinzu
            context.Schueler.Add(neuerSchueler);

            // Entity Framework erstellt das INSERT automatisch
            context.SaveChanges();

            // Nach SaveChanges enthält das Objekt die von der
            // Datenbank erstellte SchuelerId
            return neuerSchueler.SchuelerId;
        }

        public bool Bearbeiten(int schuelerId, string name, int klasseId)
        {
            using SchulAppContext context = new SchulAppContext();

            // Sucht den Schüler anhand seiner ID
            SchuelerModel? schueler = context.Schueler.Find(schuelerId);

            // Schüler existiert nicht
            if (schueler == null)
            {
                return false;
            }

            // Ändert die Daten am geladenen Objekt
            schueler.Name = name;
            schueler.KlasseId = klasseId;

            // Entity Framework erkennt die Änderungen
            // und erstellt das UPDATE automatisch
            context.SaveChanges();

            return true;
        }

        public bool Loeschen(int schuelerId)
        {
            using SchulAppContext context = new SchulAppContext();

            // Sucht zuerst den Schüler
            SchuelerModel? schueler = context.Schueler.Find(schuelerId);

            // Schüler wurde nicht gefunden
            if (schueler == null)
            {
                return false;
            }

            // Markiert den Schüler zum Löschen
            context.Schueler.Remove(schueler);

            // Entity Framework erstellt das DELETE automatisch
            context.SaveChanges();

            return true;
        }
    }
}