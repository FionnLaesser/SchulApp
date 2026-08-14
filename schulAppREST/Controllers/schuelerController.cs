using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchuelerController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public SchuelerController(SchulAppContext context)
        {
            _context = context;
        }

        //GetStudents
        [HttpGet]
        public async Task<IActionResult> GetSchueler()
        {
            var schueler = await _context.Schueler.ToListAsync();

            return Ok(schueler);
        }

        //GetStudentsWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchuelerById(int id)
        {
            var schueler = await _context.Schueler.FindAsync(id);

            if (schueler == null)
            {
                return NotFound();
            }

            return Ok(schueler);
        }
        // AddStudent
        [HttpPost]
        public async Task<IActionResult> AddSchueler([FromBody] SchuelerModel schueler)
        {
            _context.Schueler.Add(schueler);

            await _context.SaveChangesAsync();

            return Ok(schueler);
        }
        // UpdateStudent
        // UpdateStudent
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            [FromBody] SchuelerModel schueler)
        {
            var schueler1 = await _context.Schueler.FindAsync(id);

            if (schueler1 == null)
            {
                return NotFound();
            }

            // Primary Key darf nicht geändert werden
            schueler.SchuelerId = schueler1.SchuelerId;

            // Restliche Werte überschreiben
            _context.Entry(schueler1).CurrentValues.SetValues(schueler);

            await _context.SaveChangesAsync();

            return Ok(schueler1);
        }

        //DeleteStudent
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelStudent(int id)
        {
            var schueler = await _context.Schueler.FindAsync(id);

            if (schueler == null)
            {
                return NotFound();
            }

            _context.Schueler.Remove(schueler);
            await _context.SaveChangesAsync();

            return Ok(schueler);
        }
    }
 }