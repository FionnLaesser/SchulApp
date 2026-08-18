using Microsoft.EntityFrameworkCore;
using SchulAppSOAP.Contracts;
using SchulAppSOAP.Data;
using SchulAppSOAP.Models;

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

            bool klasseExistiert = await _context.Klassen
                .AnyAsync(k => k.KlassenId == schueler.KlasseId);

            if (!klasseExistiert)
            {
                return false;
            }

            schueler.Name = schueler.Name.Trim();

            _context.Schueler.Add(schueler);

            await _context.SaveChangesAsync();

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

            vorhandenerSchueler.Name = schueler.Name.Trim();
            vorhandenerSchueler.KlasseId = schueler.KlasseId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSchueler(int id)
        {
            var schueler = await _context.Schueler.FindAsync(id);

            if (schueler == null)
            {
                return false;
            }

            _context.Schueler.Remove(schueler);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}