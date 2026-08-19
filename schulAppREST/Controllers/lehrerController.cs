using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;
using schulAppREST.Services;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LehrerController : ControllerBase
    {
        private readonly SchulAppContext _context;
        private readonly AuditLogService _auditLog;

        public LehrerController(
            SchulAppContext context,
            AuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        //GetTeachers
        [HttpGet]
        public async Task<IActionResult> GetLehrer()
        {
            var lehrer = await _context.Lehrer.ToListAsync();

            return Ok(lehrer);
        }

        //GetTeachersWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLehrerById(int id)
        {
            var lehrer = await _context.Lehrer.FindAsync(id);

            if (lehrer == null)
            {
                return NotFound();
            }

            return Ok(lehrer);
        }

        // AddTeacher
        [HttpPost]
        public async Task<IActionResult> AddLehrer([FromBody] LehrerModel lehrer)
        {
            _context.Lehrer.Add(lehrer);

            await _context.SaveChangesAsync();

            await _auditLog.WriteAsync(
                "CREATE",
                "Lehrer",
                $"LehrerId={lehrer.LehrerId}",
                new
                {
                    lehrer.Name,
                    lehrer.Email,
                    lehrer.Telefon
                }
            );

            return Ok(lehrer);
        }
        // UpdateTeacher
        // UpdateTeacher
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(
            int id,
            [FromBody] LehrerModel lehrer)
        {
            var lehrer1 = await _context.Lehrer.FindAsync(id);

            if (lehrer1 == null)
            {
                return NotFound();
            }

            var changes = new
            {
                Name = new
                {
                    Old = lehrer1.Name,
                    New = lehrer.Name
                },
                Email = new
                {
                    Old = lehrer1.Email,
                    New = lehrer.Email
                },
                Telefon = new
                {
                    Old = lehrer1.Telefon,
                    New = lehrer.Telefon
                }
            };

            // Primary Key darf nicht geändert werden
            lehrer.LehrerId = lehrer1.LehrerId;

            // Restliche Werte überschreiben
            _context.Entry(lehrer1).CurrentValues.SetValues(lehrer);

            await _context.SaveChangesAsync();

            await _auditLog.WriteAsync(
                "UPDATE",
                "Lehrer",
                $"LehrerId={id}",
                changes
            );

            return Ok(lehrer1);
        }

        //DeleteTeacher
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelTeacher(int id)
        {
            var lehrer = await _context.Lehrer.FindAsync(id);

            if (lehrer == null)
            {
                return NotFound();
            }

            var deleted = new
            {
                lehrer.Name,
                lehrer.Email,
                lehrer.Telefon
            };

            _context.Lehrer.Remove(lehrer);
            await _context.SaveChangesAsync();

            await _auditLog.WriteAsync(
                "DELETE",
                "Lehrer",
                $"LehrerId={id}",
                deleted
            );

            return Ok(lehrer);
        }
    }
 }