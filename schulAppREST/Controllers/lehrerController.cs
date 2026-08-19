using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using schulAppREST.Services;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LehrerController : ControllerBase
    {
        private readonly SchulAppContext _context;
        private readonly AuditLogService _auditLog;

        public LehrerController(SchulAppContext context, AuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        [HttpGet]
        public async Task<IActionResult> GetLehrer()
        {
            return Ok(await _context.Lehrer.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLehrerById(int id)
        {
            var lehrer = await _context.Lehrer.FindAsync(id);
            return lehrer == null ? NotFound() : Ok(lehrer);
        }

        [HttpPost]
        public async Task<IActionResult> AddLehrer([FromBody] LehrerModel lehrer)
        {
            _context.Lehrer.Add(lehrer);
            await _context.SaveChangesAsync();
            await _auditLog.WriteAsync(
                "CREATE", "Lehrer", $"LehrerId={lehrer.LehrerId}",
                new { lehrer.Name, lehrer.Email, lehrer.Telefon });
            return Ok(lehrer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] LehrerModel lehrer)
        {
            var existing = await _context.Lehrer.FindAsync(id);
            if (existing == null) return NotFound();

            var changes = new
            {
                Name = new { Old = existing.Name, New = lehrer.Name },
                Email = new { Old = existing.Email, New = lehrer.Email },
                Telefon = new { Old = existing.Telefon, New = lehrer.Telefon }
            };

            lehrer.LehrerId = existing.LehrerId;
            _context.Entry(existing).CurrentValues.SetValues(lehrer);
            await _context.SaveChangesAsync();
            await _auditLog.WriteAsync("UPDATE", "Lehrer", $"LehrerId={id}", changes);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DelTeacher(int id)
        {
            var lehrer = await _context.Lehrer.FindAsync(id);
            if (lehrer == null) return NotFound();

            var deleted = new { lehrer.Name, lehrer.Email, lehrer.Telefon };
            _context.Lehrer.Remove(lehrer);
            await _context.SaveChangesAsync();
            await _auditLog.WriteAsync("DELETE", "Lehrer", $"LehrerId={id}", deleted);
            return Ok(lehrer);
        }
    }
}
