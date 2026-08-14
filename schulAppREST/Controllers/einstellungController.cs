using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

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

        // GET: api/Einstellung
        [HttpGet]
        public async Task<IActionResult> GetEinstellungen()
        {
            var einstellungen = await _context.Einstellung.ToListAsync();

            return Ok(einstellungen);
        }

        // GET: api/Einstellung/1
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

        // POST: api/Einstellung
        [HttpPost]
        public async Task<IActionResult> AddEinstellung(
            [FromBody] EinstellungModel einstellung)
        {
            _context.Einstellung.Add(einstellung);

            await _context.SaveChangesAsync();

            return Ok(einstellung);
        }

        // PUT: api/Einstellung/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEinstellung(
            int id,
            [FromBody] EinstellungModel einstellung)
        {
            var vorhandeneEinstellung =
                await _context.Einstellung.FindAsync(id);

            if (vorhandeneEinstellung == null)
            {
                return NotFound();
            }

            vorhandeneEinstellung.HintergrundFarbe =
                einstellung.HintergrundFarbe;

            vorhandeneEinstellung.TextFarbe =
                einstellung.TextFarbe;

            await _context.SaveChangesAsync();

            return Ok(vorhandeneEinstellung);
        }

        // DELETE: api/Einstellung/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEinstellung(int id)
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