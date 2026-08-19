using Microsoft.EntityFrameworkCore;
using SchulApp.Models;

namespace SchulApp.Data
{
    public class SchulAppContext : DbContext
    {
        public SchulAppContext() { }

        public SchulAppContext(DbContextOptions<SchulAppContext> options)
            : base(options) { }

        public DbSet<KlasseModel> Klasse { get; set; }
        public DbSet<LehrerModel> Lehrer { get; set; }
        public DbSet<SchuelerModel> Schueler { get; set; }
        public DbSet<LehrerInformationModel> LehrerInformationen { get; set; }
        public DbSet<EinstellungModel> Einstellung { get; set; }
        public DbSet<KursModel> Kurs { get; set; }
        public DbSet<StundenplanModel> Stundenplan { get; set; }
        public DbSet<LoginBenutzer> Benutzer { get; set; }
        public DbSet<PingPongSpiel> PingPongSpiel { get; set; }
        public DbSet<AuditLogModel> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchulAppContext).Assembly);
            modelBuilder.Entity<AuditLogModel>().HasIndex(x => x.TimestampUtc);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=localhost;Database=SchulAppDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
                );
            }
        }
    }
}
