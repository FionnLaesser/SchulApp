using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KursController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public KursController(SchulAppContext context)
        {
            _context = context;
        }

        //GetStudents
        [HttpGet]
        public async Task<IActionResult> GetKurs()
        {
            var kurs = await _context.Kurs.ToListAsync();

            return Ok(kurs);
        }

        //GetStudentsWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKursById(int id)
        {
            var kurs = await _context.Kurs.FindAsync(id);

            if (kurs == null)
            {
                return NotFound();
            }

            return Ok(kurs);
        }

        // AddStudent
        [HttpPost]
        public async Task<IActionResult> AddKurs([FromBody] KursModel kurs)
        {
            _context.Kurs.Add(kurs);

            await _context.SaveChangesAsync();

            return Ok(kurs);
        }
        // UpdateStudent
        // UpdateStudent
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            [FromBody] KursModel kurs)
        {
            var kurs1 = await _context.Kurs.FindAsync(id);

            if (kurs1 == null)
            {
                return NotFound();
            }

            // Primary Key darf nicht geändert werden
            kurs.KursId = kurs1.KursId;

            // Restliche Werte überschreiben
            _context.Entry(kurs1).CurrentValues.SetValues(kurs);

            await _context.SaveChangesAsync();

            return Ok(kurs1);
        }

        //DeleteStudent
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelStudent(int id)
        {
            var kurs = await _context.Kurs.FindAsync(id);

            if (kurs == null)
            {
                return NotFound();
            }

            _context.Kurs.Remove(kurs);
            await _context.SaveChangesAsync();

            return Ok(kurs);
        }
    }
 }