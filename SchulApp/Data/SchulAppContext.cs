using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SchulApp.Models;
using System.Text.Json;

namespace SchulApp.Data
{
    public class SchulAppContext : DbContext
    {
        private static readonly HashSet<string> SensitivePropertyNames =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "PasswortHash",
                "Profilbild"
            };

        private static readonly HashSet<string> PingPongPropertyNames =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "PingPongSiege",
                "PingPongPunkte",
                "PingPongToreErzielt",
                "PingPongToreKassiert"
            };

        public SchulAppContext()
        {
        }

        public SchulAppContext(DbContextOptions<SchulAppContext> options)
            : base(options)
        {
        }

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

        // Tabelle dbo.AuditLog
        public DbSet<AuditLogModel> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            SicherstellenDassAuditLogsNichtVeraendertWerden();

            List<PendingAuditLog> pendingLogs = AuditLogsVorbereiten();
            int result = base.SaveChanges();

            AuditLogsSicherSpeichern(pendingLogs);

            return result;
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SicherstellenDassAuditLogsNichtVeraendertWerden();

            List<PendingAuditLog> pendingLogs = AuditLogsVorbereiten();
            int result = await base.SaveChangesAsync(cancellationToken);

            await AuditLogsSicherSpeichernAsync(
                pendingLogs,
                cancellationToken
            );

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(SchulAppContext).Assembly
            );

            modelBuilder.Entity<AuditLogModel>()
                .HasIndex(x => x.TimestampUtc);
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
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

        private void SicherstellenDassAuditLogsNichtVeraendertWerden()
        {
            bool wirdVeraendert = ChangeTracker
                .Entries<AuditLogModel>()
                .Any(x =>
                    x.State == EntityState.Modified ||
                    x.State == EntityState.Deleted
                );

            if (wirdVeraendert)
            {
                throw new InvalidOperationException(
                    "Audit-Log-Einträge dürfen nicht geändert oder gelöscht werden."
                );
            }
        }

        private List<PendingAuditLog> AuditLogsVorbereiten()
        {
            ChangeTracker.DetectChanges();

            return ChangeTracker
                .Entries()
                .Where(SollProtokolliertWerden)
                .Select(AuditLogErstellen)
                .ToList();
        }

        private bool SollProtokolliertWerden(EntityEntry entry)
        {
            if (entry.Entity is AuditLogModel)
            {
                return false;
            }

            if (entry.State != EntityState.Added &&
                entry.State != EntityState.Modified &&
                entry.State != EntityState.Deleted)
            {
                return false;
            }

            if (entry.Metadata.ClrType.Name == "PingPongSpiel")
            {
                return false;
            }

            if (entry.Entity is LoginBenutzer &&
                entry.State == EntityState.Modified)
            {
                List<string> geaenderteEigenschaften = entry.Properties
                    .Where(x => x.IsModified)
                    .Select(x => x.Metadata.Name)
                    .ToList();

                if (geaenderteEigenschaften.Count > 0 &&
                    geaenderteEigenschaften.All(
                        x => PingPongPropertyNames.Contains(x)
                    ))
                {
                    return false;
                }
            }

            return true;
        }

        private PendingAuditLog AuditLogErstellen(EntityEntry entry)
        {
            string action = entry.State switch
            {
                EntityState.Added => "CREATE",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted => "DELETE",
                _ => "UNKNOWN"
            };

            string entityType =
                entry.Metadata.GetTableName() ??
                entry.Metadata.ClrType.Name;

            string? entityId = entry.State == EntityState.Added
                ? null
                : EntityIdLesen(entry);

            string? changes = AenderungenSerialisieren(entry);

            string userName =
                string.IsNullOrWhiteSpace(BenutzerSession.Benutzername)
                    ? "System"
                    : BenutzerSession.Benutzername;

            string? userRole =
                string.IsNullOrWhiteSpace(BenutzerSession.Rolle)
                    ? "System"
                    : BenutzerSession.Rolle;

            return new PendingAuditLog
            {
                Entry = entry,
                Log = new AuditLogModel
                {
                    TimestampUtc = DateTime.UtcNow,
                    UserName = userName,
                    UserRole = userRole,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Changes = changes,
                    Source = "WinForms"
                }
            };
        }

        private static string? EntityIdLesen(EntityEntry entry)
        {
            var primaryKey = entry.Metadata.FindPrimaryKey();

            if (primaryKey == null)
            {
                return null;
            }

            List<string> teile = new List<string>();

            foreach (var keyProperty in primaryKey.Properties)
            {
                PropertyEntry property = entry.Property(keyProperty.Name);
                object? value = property.CurrentValue ?? property.OriginalValue;

                teile.Add($"{keyProperty.Name}={value}");
            }

            return string.Join(", ", teile);
        }

        private static string? AenderungenSerialisieren(EntityEntry entry)
        {
            if (entry.State == EntityState.Modified)
            {
                Dictionary<string, object?> changes =
                    new Dictionary<string, object?>();

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (!property.IsModified ||
                        SensitivePropertyNames.Contains(
                            property.Metadata.Name
                        ))
                    {
                        continue;
                    }

                    changes[property.Metadata.Name] = new
                    {
                        Old = property.OriginalValue,
                        New = property.CurrentValue
                    };
                }

                return changes.Count == 0
                    ? null
                    : JsonSerializer.Serialize(changes);
            }

            Dictionary<string, object?> values =
                new Dictionary<string, object?>();

            foreach (PropertyEntry property in entry.Properties)
            {
                if (SensitivePropertyNames.Contains(
                    property.Metadata.Name
                ))
                {
                    continue;
                }

                bool istPrimaryKey =
                    entry.Metadata.FindPrimaryKey()?.Properties
                        .Contains(property.Metadata) == true;

                if (entry.State == EntityState.Added && istPrimaryKey)
                {
                    continue;
                }

                values[property.Metadata.Name] =
                    entry.State == EntityState.Deleted
                        ? property.OriginalValue
                        : property.CurrentValue;
            }

            return values.Count == 0
                ? null
                : JsonSerializer.Serialize(values);
        }

        private void AuditLogsSicherSpeichern(
            List<PendingAuditLog> pendingLogs)
        {
            if (pendingLogs.Count == 0)
            {
                return;
            }

            try
            {
                foreach (PendingAuditLog pending in pendingLogs)
                {
                    if (pending.Log.EntityId == null)
                    {
                        pending.Log.EntityId =
                            EntityIdLesen(pending.Entry);
                    }
                }

                AuditLogs.AddRange(
                    pendingLogs.Select(x => x.Log)
                );

                base.SaveChanges();
            }
            catch (Exception ex)
            {
                AuditLogEntriesEntfernen();
                System.Diagnostics.Debug.WriteLine(
                    $"Audit logging failed: {ex.Message}"
                );
            }
        }

        private async Task AuditLogsSicherSpeichernAsync(
            List<PendingAuditLog> pendingLogs,
            CancellationToken cancellationToken)
        {
            if (pendingLogs.Count == 0)
            {
                return;
            }

            try
            {
                foreach (PendingAuditLog pending in pendingLogs)
                {
                    if (pending.Log.EntityId == null)
                    {
                        pending.Log.EntityId =
                            EntityIdLesen(pending.Entry);
                    }
                }

                AuditLogs.AddRange(
                    pendingLogs.Select(x => x.Log)
                );

                await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                AuditLogEntriesEntfernen();
                System.Diagnostics.Debug.WriteLine(
                    $"Audit logging failed: {ex.Message}"
                );
            }
        }

        private void AuditLogEntriesEntfernen()
        {
            foreach (EntityEntry<AuditLogModel> entry in
                     ChangeTracker.Entries<AuditLogModel>()
                         .Where(x => x.State == EntityState.Added)
                         .ToList())
            {
                entry.State = EntityState.Detached;
            }
        }

        private sealed class PendingAuditLog
        {
            public required EntityEntry Entry { get; init; }

            public required AuditLogModel Log { get; init; }
        }
    }
}
