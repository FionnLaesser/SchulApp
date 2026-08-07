using SchulApp.Models;

namespace SchulApp.Data
{
    public interface ISchuelerRepository
    {
        IReadOnlyList<SchuelerModel> AlleLaden();
        int Erstellen(string name, int klasseId);
        bool Bearbeiten(int schuelerId, string name, int klasseId);
        bool Loeschen(int schuelerId);
    }
}
