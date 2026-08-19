using Microsoft.EntityFrameworkCore;
using SchulApp.Models;

namespace SchulApp.Data
{
    public class SchulAppContext : DbContext
    {
        public SchulAppContext()
        {
        }

        public SchulAppContext(DbContextOptions<SchulAppContext> options)
            : base(options)
        {
        }

        // Tabelle dbo.Klasse
        public DbSet<KlasseModel> Klasse { get; set; }

        // Tabelle dbo.Lehrer
        public DbSet<LehrerModel> Lehrer { get; set; }

        // Tabelle dbo.Schueler
        public DbSet<SchuelerModel> Schueler { get; set; }

        // Tabelle dbo.LehrerInformationen
        public DbSet<LehrerInformationModel> LehrerInformationen { get; set; }

        // Tabelle dbo.Einstellung
        public DbSet<EinstellungModel> Einstellung {  get; set; }

        // Tabelle dbo.Kurs
        public DbSet<KursModel> Kurs { get; set; }

        // Tabelle dbo.Login
        //  public DbSet<LoginBenutzer> Login { get; set; }

        // Tabelle dbo.Stundenplan
        public DbSet<StundenplanModel> Stundenplan { get; set; }

        // Tabelle dbo.Benutzer
        public DbSet<LoginBenutzer> Benutzer { get; set; }

        // Tabelle dbo.PingPongSpiel
        public DbSet<PingPongSpiel> PingPongSpiel { get; set; }

        // Tabelle dbo.AuditLog
        public DbSet<AuditLogModel> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(SchulAppContext).Assembly
            );

            modelBuilder.Entity<AuditLogModel>()
                .HasIndex(x => x.TimestampUtc);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=localhost;" +
                    "Database=SchulAppDB;" +
                    "Integrated Security=True;" +
                    "Encrypt=True;" +
                    "TrustServerCertificate=True;"
                );
            }
        }
    }
}