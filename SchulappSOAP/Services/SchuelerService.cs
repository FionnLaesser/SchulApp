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
            return await _context.Schueler.ToListAsync();
        }

        public async Task<SchuelerModel?> GetSchuelerById(int id)
        {
            return await _context.Schueler.FindAsync(id);
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