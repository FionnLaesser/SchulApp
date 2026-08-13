using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Models;
using SchulApp.Data;
using Microsoft.Identity.Client.NativeInterop;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StundenplanController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public StundenplanController(SchulAppContext context)
        {
            _context = context;
        }

        //GetStudents
        [HttpGet]
        public async Task<IActionResult> GetStundenplan()
        {
            var stundenplan = await _context.Stundenplan.ToListAsync();

            return Ok(stundenplan);
        }

        //GetStudentsWithID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStundenplanById(int id)
        {
            var stundenplan = await _context.Stundenplan.FindAsync(id);

            if (stundenplan == null)
            {
                return NotFound();
            }

            return Ok(stundenplan);
        }

        // AddStudent
        [HttpPost]
        public async Task<IActionResult> AddStundenplan([FromBody] StundenplanModel stundenplan)
        {
            _context.Stundenplan.Add(stundenplan);

            await _context.SaveChangesAsync();

            return Ok(stundenplan);
        }
        // UpdateStudent
        // UpdateStudent
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            [FromBody] StundenplanModel stundenplan)
        {
            var stundenplan1 = await _context.Stundenplan.FindAsync(id);

            if (stundenplan1 == null)
            {
                return NotFound();
            }

            // Primary Key darf nicht geändert werden
            stundenplan.StundenplanId = stundenplan1.StundenplanId;

            // Restliche Werte überschreiben
            _context.Entry(stundenplan1).CurrentValues.SetValues(stundenplan);

            await _context.SaveChangesAsync();

            return Ok(stundenplan1);
        }

        //DeleteStudent
        [HttpDelete("{id}")]
        public async Task<IActionResult> DelStudent(int id)
        {
            var stundenplan = await _context.Stundenplan.FindAsync(id);

            if (stundenplan == null)
            {
                return NotFound();
            }

            _context.Stundenplan.Remove(stundenplan);
            await _context.SaveChangesAsync();

            return Ok(stundenplan);
        }
    }
 }