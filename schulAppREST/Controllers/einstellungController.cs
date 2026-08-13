using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EinstellungController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public EinstellungController(SchulAppContext context)
        {
            _context = context;
        }

        //GetStudents
        [HttpGet]
        public async Task<IActionResult> GetEinstellung()
        {
            var einstellung = await _context.Einstellung.ToListAsync();

            return Ok(einstellung);
        }

        //GetStudentsWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEinstellungById(int id)
        {
            var einstellung = await _context.Einstellung.FindAsync(id);

            if (einstellung == null)
            {
                return NotFound();
            }

            return Ok(einstellung);
        }

        // AddStudent
        [HttpPost]
        public async Task<IActionResult> AddEinstellung([FromBody] EinstellungModel einstellung)
        {
            _context.Einstellung.Add(einstellung);

            await _context.SaveChangesAsync();

            return Ok(einstellung);
        }
        // UpdateStudent
        // UpdateStudent
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            [FromBody] EinstellungModel einstellung)
        {
            var einstellung1 = await _context.Einstellung.FindAsync(id);

            if (einstellung1 == null)
            {
                return NotFound();
            }

            // Primary Key darf nicht geändert werden
            einstellung.Id = einstellung1.Id;

            // Restliche Werte überschreiben
            _context.Entry(einstellung1).CurrentValues.SetValues(einstellung);

            await _context.SaveChangesAsync();

            return Ok(einstellung1);
        }

        //DeleteStudent
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelStudent(int id)
        {
            var einstellung = await _context.Einstellung.FindAsync(id);

            if (einstellung == null)
            {
                return NotFound();
            }

            _context.Einstellung.Remove(einstellung);
            await _context.SaveChangesAsync();

            return Ok(einstellung);
        }
    }
 }