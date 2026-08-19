using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Contracts;
using SchulAppSOAP.Data;
using SchulAppSOAP.Models;
using System.Text.Json;

namespace SchulAppSOAP.Services
{
    public class SchuelerService : ISchuelerService
    {
        private readonly SchulAppContext _context;

        public SchuelerService(SchulAppContext context)
        {
            _context = context;
        }

        public async Task<List<SchuelerModel>> GetSchueler()
        {
            return await _context.Schueler
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SchuelerModel?> GetSchuelerById(int id)
        {
            return await _context.Schueler
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SchuelerId == id);
        }

        public async Task<bool> AddSchueler(SchuelerModel schueler)
        {
            if (schueler == null ||
                string.IsNullOrWhiteSpace(schueler.Name))
            {
                return false;
            }

            string? auditUserName = schueler.AuditUserName;
            string? auditUserRole = schueler.AuditUserRole;

            bool klasseExistiert = await _context.Klassen
                .AnyAsync(k => k.KlassenId == schueler.KlasseId);

            if (!klasseExistiert)
            {
                return false;
            }

            schueler.Name = schueler.Name.Trim();
            schueler.AuditUserName = null;
            schueler.AuditUserRole = null;

            _context.Schueler.Add(schueler);

            await _context.SaveChangesAsync();

            await AuditLogSicherSpeichernAsync(
                "CREATE",
                schueler.SchuelerId,
                new
                {
                    schueler.Name,
                    schueler.KlasseId
                },
                auditUserName,
                auditUserRole
            );

            return true;
        }

        public async Task<bool> UpdateSchueler(SchuelerModel schueler)
        {
            if (schueler == null ||
                schueler.SchuelerId <= 0 ||
                string.IsNullOrWhiteSpace(schueler.Name))
            {
                return false;
            }

            string? auditUserName = schueler.AuditUserName;
            string? auditUserRole = schueler.AuditUserRole;

            var vorhandenerSchueler = await _context.Schueler
                .FindAsync(schueler.SchuelerId);

            if (vorhandenerSchueler == null)
            {
                return false;
            }

            bool klasseExistiert = await _context.Klassen
                .AnyAsync(k => k.KlassenId == schueler.KlasseId);

            if (!klasseExistiert)
            {
                return false;
            }

            string alterName = vorhandenerSchueler.Name;
            int alteKlasseId = vorhandenerSchueler.KlasseId;
            string neuerName = schueler.Name.Trim();

            Dictionary<string, object?> changes =
                new Dictionary<string, object?>();

            if (!string.Equals(
                    alterName,
                    neuerName,
                    StringComparison.Ordinal))
            {
                changes["Name"] = new
                {
                    Old = alterName,
                    New = neuerName
                };
            }

            if (alteKlasseId != schueler.KlasseId)
            {
                changes["KlasseId"] = new
                {
                    Old = alteKlasseId,
                    New = schueler.KlasseId
                };
            }

            vorhandenerSchueler.Name = neuerName;
            vorhandenerSchueler.KlasseId = schueler.KlasseId;

            await _context.SaveChangesAsync();

            if (changes.Count > 0)
            {
                await AuditLogSicherSpeichernAsync(
                    "UPDATE",
                    vorhandenerSchueler.SchuelerId,
                    changes,
                    auditUserName,
                    auditUserRole
                );
            }

            return true;
        }

        public async Task<bool> DeleteSchueler(int id)
        {
            return await DeleteSchuelerInternal(
                id,
                null,
                null
            );
        }

        public async Task<bool> DeleteSchuelerMitAudit(
            int id,
            string? auditUserName,
            string? auditUserRole)
        {
            return await DeleteSchuelerInternal(
                id,
                auditUserName,
                auditUserRole
            );
        }

        private async Task<bool> DeleteSchuelerInternal(
            int id,
            string? auditUserName,
            string? auditUserRole)
        {
            var schueler = await _context.Schueler.FindAsync(id);

            if (schueler == null)
            {
                return false;
            }

            string name = schueler.Name;
            int klasseId = schueler.KlasseId;

            _context.Schueler.Remove(schueler);

            await _context.SaveChangesAsync();

            await AuditLogSicherSpeichernAsync(
                "DELETE",
                id,
                new
                {
                    Name = name,
                    KlasseId = klasseId
                },
                auditUserName,
                auditUserRole
            );

            return true;
        }

        private async Task AuditLogSicherSpeichernAsync(
            string action,
            int schuelerId,
            object? changes,
            string? auditUserName,
            string? auditUserRole)
        {
            string userName =
                string.IsNullOrWhiteSpace(auditUserName)
                    ? "System"
                    : auditUserName.Trim();

            string userRole =
                string.IsNullOrWhiteSpace(auditUserRole)
                    ? "System"
                    : auditUserRole.Trim();

            AuditLogModel auditLog = new AuditLogModel
            {
                TimestampUtc = DateTime.UtcNow,
                UserName = userName,
                UserRole = userRole,
                Action = action,
                EntityType = "Schueler",
                EntityId = $"SchuelerId={schuelerId}",
                Changes = changes == null
                    ? null
                    : JsonSerializer.Serialize(changes),
                Source = "SOAP"
            };

            try
            {
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (_context.Entry(auditLog).State != EntityState.Detached)
                {
                    _context.Entry(auditLog).State = EntityState.Detached;
                }

                System.Diagnostics.Debug.WriteLine(
                    $"SOAP audit logging failed: {ex.Message}"
                );
            }
        }
    }
}
