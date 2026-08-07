using Microsoft.Data.SqlClient;
using SchulApp.Models;
using System.Data;

namespace SchulApp.Data
{
    public class SqlSchuelerRepository : ISchuelerRepository
    {
        public IReadOnlyList<SchuelerModel> AlleLaden()
        {
            const string sql = @"
                SELECT
                    s.SchuelerId,
                    s.Name,
                    s.KlasseId,
                    k.Bezeichnung AS Klasse
                FROM dbo.Schueler AS s
                INNER JOIN dbo.Klassen AS k
                    ON s.KlasseId = k.KlassenId
                ORDER BY s.Name;";

            List<SchuelerModel> ergebnis = new List<SchuelerModel>();

            using SqlConnection connection = Database.GetConnection();
            using SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ergebnis.Add(new SchuelerModel
                {
                    SchuelerId = reader.GetInt32(reader.GetOrdinal("SchuelerId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    KlasseId = reader.GetInt32(reader.GetOrdinal("KlasseId")),
                    Klasse = reader.GetString(reader.GetOrdinal("Klasse"))
                });
            }

            return ergebnis;
        }

        public int Erstellen(string name, int klasseId)
        {
            const string sql = @"
                INSERT INTO dbo.Schueler (Name, KlasseId)
                OUTPUT INSERTED.SchuelerId
                VALUES (@Name, @KlasseId);";

            using SqlConnection connection = Database.GetConnection();
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
            command.Parameters.Add("@KlasseId", SqlDbType.Int).Value = klasseId;

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Bearbeiten(int schuelerId, string name, int klasseId)
        {
            const string sql = @"
                UPDATE dbo.Schueler
                SET Name = @Name,
                    KlasseId = @KlasseId
                WHERE SchuelerId = @SchuelerId;";

            using SqlConnection connection = Database.GetConnection();
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
            command.Parameters.Add("@KlasseId", SqlDbType.Int).Value = klasseId;
            command.Parameters.Add("@SchuelerId", SqlDbType.Int).Value = schuelerId;

            connection.Open();
            return command.ExecuteNonQuery() == 1;
        }

        public bool Loeschen(int schuelerId)
        {
            const string sql = @"
                DELETE FROM dbo.Schueler
                WHERE SchuelerId = @SchuelerId;";

            using SqlConnection connection = Database.GetConnection();
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@SchuelerId", SqlDbType.Int).Value = schuelerId;

            connection.Open();
            return command.ExecuteNonQuery() == 1;
        }
    }
}
