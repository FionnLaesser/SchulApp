using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp.Services
{
    public class SchuelerService
    {
        private readonly ISchuelerRepository repository;

        public SchuelerService(ISchuelerRepository repository)
        {
            this.repository = repository;
        }

        public IReadOnlyList<SchuelerModel> AlleLaden()
        {
            return repository.AlleLaden();
        }

        public int Erstellen(string name, int klasseId)
        {
            name = NamePruefen(name);
            KlassePruefen(klasseId);
            return repository.Erstellen(name, klasseId);
        }

        public bool Bearbeiten(int schuelerId, string name, int klasseId)
        {
            if (schuelerId <= 0)
            {
                throw new ArgumentException("Die Schüler-ID ist ungültig.");
            }

            name = NamePruefen(name);
            KlassePruefen(klasseId);
            return repository.Bearbeiten(schuelerId, name, klasseId);
        }

        public bool Loeschen(int schuelerId)
        {
            if (schuelerId <= 0)
            {
                throw new ArgumentException("Die Schüler-ID ist ungültig.");
            }

            return repository.Loeschen(schuelerId);
        }

        private static string NamePruefen(string name)
        {
            string bereinigt = (name ?? "").Trim();

            if (bereinigt.Length == 0)
            {
                throw new ArgumentException("Bitte einen Namen eingeben.");
            }

            if (bereinigt.Length > 100)
            {
                throw new ArgumentException("Der Name darf maximal 100 Zeichen lang sein.");
            }

            return bereinigt;
        }

        private static void KlassePruefen(int klasseId)
        {
            if (klasseId <= 0)
            {
                throw new ArgumentException("Bitte eine Klasse auswählen.");
            }
        }
    }
}
