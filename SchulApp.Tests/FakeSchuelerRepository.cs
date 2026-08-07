using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp.Tests
{
    internal class FakeSchuelerRepository : ISchuelerRepository
    {
        private readonly List<SchuelerModel> schueler = new List<SchuelerModel>();
        private int naechsteId = 1;

        public IReadOnlyList<SchuelerModel> AlleLaden()
        {
            return schueler
                .Select(s => new SchuelerModel
                {
                    SchuelerId = s.SchuelerId,
                    Name = s.Name,
                    KlasseId = s.KlasseId,
                    Klasse = s.Klasse
                })
                .ToList();
        }

        public int Erstellen(string name, int klasseId)
        {
            SchuelerModel neuerSchueler = new SchuelerModel
            {
                SchuelerId = naechsteId++,
                Name = name,
                KlasseId = klasseId,
                Klasse = "Testklasse"
            };

            schueler.Add(neuerSchueler);
            return neuerSchueler.SchuelerId;
        }

        public bool Bearbeiten(int schuelerId, string name, int klasseId)
        {
            SchuelerModel gefunden = schueler.FirstOrDefault(s => s.SchuelerId == schuelerId);
            if (gefunden == null)
            {
                return false;
            }

            gefunden.Name = name;
            gefunden.KlasseId = klasseId;
            return true;
        }

        public bool Loeschen(int schuelerId)
        {
            SchuelerModel gefunden = schueler.FirstOrDefault(s => s.SchuelerId == schuelerId);
            if (gefunden == null)
            {
                return false;
            }

            schueler.Remove(gefunden);
            return true;
        }
    }
}
