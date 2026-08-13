using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlasseController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public KlasseController(SchulAppContext context)
        {
            _context = context;
        }

        //GetTeachers
        [HttpGet]
        public async Task<IActionResult> GetKlasse()
        {
            var klasse = await _context.Klasse.ToListAsync();

            return Ok(klasse);
        }

        //GetTeachersWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKlasseById(int id)
        {
            var klasse = await _context.Klasse.FindAsync(id);

            if (klasse == null)
            {
                return NotFound();
            }

            return Ok(klasse);
        }

        // AddTeacher
        [HttpPost]
        public async Task<IActionResult> AddKlasse([FromBody] KlasseModel klasse)
        {
            _context.Klasse.Add(klasse);

            await _context.SaveChangesAsync();

            return Ok(klasse);
        }
        // UpdateTeacher
        // UpdateTeacher
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(
            int id,
            [FromBody] KlasseModel klasse)
        {
            var klasse1 = await _context.Klasse.FindAsync(id);

            if (klasse1 == null)
            {
                return NotFound();
            }

            // Primary Key darf nicht geändert werden
            klasse.KlassenId = klasse1.KlassenId;

            // Restliche Werte überschreiben
            _context.Entry(klasse1).CurrentValues.SetValues(klasse);
            await _context.SaveChangesAsync();

            return Ok(klasse1);
        }

        //DeleteTeacher
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelTeacher(int id)
        {
            var klasse = await _context.Klasse.FindAsync(id);

            if (klasse == null)
            {
                return NotFound();
            }

            _context.Klasse.Remove(klasse);
            await _context.SaveChangesAsync();

            return Ok(klasse);
        }
    }
 }