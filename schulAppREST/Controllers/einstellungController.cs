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
        private const int StandardBallSpeed = 6;
        private const int MinBallSpeed = 1;
        private const int MaxBallSpeed = 20;

        private readonly SchulAppContext _context;

        public EinstellungController(SchulAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEinstellungen()
        {
            var einstellungen = await _context.Einstellung.ToListAsync();

            return Ok(einstellungen);
        }

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

        [HttpPost]
        public async Task<IActionResult> AddEinstellung(
            [FromBody] EinstellungModel einstellung)
        {
            if (!BallSpeedIstGueltig(einstellung.BallSpeed))
            {
                return BadRequest(
                    $"BallSpeed muss zwischen {MinBallSpeed} und {MaxBallSpeed} liegen."
                );
            }

            _context.Einstellung.Add(einstellung);

            await _context.SaveChangesAsync();

            return Ok(einstellung);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEinstellung(
            int id,
            [FromBody] EinstellungModel einstellung)
        {
            if (!BallSpeedIstGueltig(einstellung.BallSpeed))
            {
                return BadRequest(
                    $"BallSpeed muss zwischen {MinBallSpeed} und {MaxBallSpeed} liegen."
                );
            }

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

            vorhandeneEinstellung.BallSpeed =
                einstellung.BallSpeed;

            await _context.SaveChangesAsync();

            return Ok(vorhandeneEinstellung);
        }

        [HttpGet("{id}/ball-speed")]
        public async Task<IActionResult> GetBallSpeed(int id)
        {
            EinstellungModel? einstellung =
                await _context.Einstellung
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == id);

            return Ok(new
            {
                BallSpeed = einstellung?.BallSpeed ?? StandardBallSpeed
            });
        }

        [HttpPut("{id}/ball-speed")]
        public async Task<IActionResult> UpdateBallSpeed(
            int id,
            [FromBody] BallSpeedAnfrage anfrage)
        {
            if (!BallSpeedIstGueltig(anfrage.BallSpeed))
            {
                return BadRequest(
                    $"BallSpeed muss zwischen {MinBallSpeed} und {MaxBallSpeed} liegen."
                );
            }

            EinstellungModel? einstellung =
                await _context.Einstellung.FindAsync(id);

            if (einstellung == null)
            {
                einstellung = new EinstellungModel
                {
                    Id = id,
                    HintergrundFarbe = anfrage.HintergrundFarbe,
                    TextFarbe = anfrage.TextFarbe,
                    BallSpeed = anfrage.BallSpeed
                };

                _context.Einstellung.Add(einstellung);
            }
            else
            {
                einstellung.BallSpeed = anfrage.BallSpeed;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                einstellung.Id,
                einstellung.BallSpeed
            });
        }

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

        private static bool BallSpeedIstGueltig(int ballSpeed)
        {
            return ballSpeed >= MinBallSpeed &&
                   ballSpeed <= MaxBallSpeed;
        }
    }

    public sealed class BallSpeedAnfrage
    {
        public int BallSpeed { get; set; }

        public int HintergrundFarbe { get; set; }

        public int TextFarbe { get; set; }
    }
}
