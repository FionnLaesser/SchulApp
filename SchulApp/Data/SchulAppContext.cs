using Microsoft.EntityFrameworkCore;
using SchulApp.Models;

namespace SchulApp.Data
{
    public class SchulAppContext : DbContext
    {
        // Tabelle dbo.Einstellung
        public DbSet<Einstellung> Einstellungen { get; set; }

        // Tabelle dbo.Klassen
        public DbSet<KlasseModel> Klassen { get; set; }

        // Tabelle dbo.Lehrer
        public DbSet<LehrerModel> Lehrer { get; set; }

        // Tabelle dbo.Schueler
        public DbSet<SchuelerModel> Schueler { get; set; }

        // Tabelle dbo.LehrerInformationen
        public DbSet<LehrerInformationModel> LehrerInformationen { get; set; }

        // Tabelle dbo.Kurse
        public DbSet<KursModel> Kurse { get; set; }

        // Tabelle dbo.Stundenplan
        public DbSet<StundenplanModel> Stundenplan { get; set; }

        // Tabelle dbo.Benutzer
        public DbSet<LoginBenutzer> Benutzer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(SchulAppContext).Assembly
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
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