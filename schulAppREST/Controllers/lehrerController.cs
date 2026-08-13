using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LehrerController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public LehrerController(SchulAppContext context)
        {
            _context = context;
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

            // Primary Key darf nicht geändert werden
            lehrer.LehrerId = lehrer1.LehrerId;

            // Restliche Werte überschreiben
            _context.Entry(lehrer1).CurrentValues.SetValues(lehrer);

            await _context.SaveChangesAsync();

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

            _context.Lehrer.Remove(lehrer);
            await _context.SaveChangesAsync();

            return Ok(lehrer);
        }
    }
 }