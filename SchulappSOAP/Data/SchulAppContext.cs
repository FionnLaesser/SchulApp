using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Models;

namespace SchulAppSOAP.Data
{
    public class SchulAppContext : DbContext
    {
        public SchulAppContext(DbContextOptions<SchulAppContext> options)
            : base(options)
        {
        }

        public DbSet<SchuelerModel> Schueler { get; set; }
        public DbSet<KlasseModel> Klassen { get; set; }
        public DbSet<AuditLogModel> AuditLogs { get; set; }
    }
}
