using Microsoft.Data.SqlClient;

namespace SchulApp
{
    public static class Database
    {
        private const string ConnectionString =
            "Server=localhost;" +
            "Database=SchulAppDB;" +
            "Integrated Security=True;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
