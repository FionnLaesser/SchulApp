using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace schulAppREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingPongController : ControllerBase
    {
        private readonly SchulAppContext _context;

        public PingPongController(SchulAppContext context)
        {
            _context = context;
        }

        // GET: api/PingPong/bestenliste
        [HttpGet("bestenliste")]
        public async Task<IActionResult> GetBestenliste()
        {
            var bestenliste = await _context.Benutzer
                .AsNoTracking()
                .Select(x => new BestenlisteEintragDto
                {
                    BenutzerId = x.Id,
                    Benutzername = x.Benutzername,
                    Siege = x.PingPongSiege,
                    Punkte = x.PingPongPunkte,
                    ToreErzielt = x.PingPongToreErzielt,
                    ToreKassiert = x.PingPongToreKassiert,
                    Torverhaeltnis =
                        x.PingPongToreErzielt - x.PingPongToreKassiert
                })
                .OrderByDescending(x => x.Punkte)
                .ThenByDescending(x => x.Torverhaeltnis)
                .ThenByDescending(x => x.ToreErzielt)
                .ThenBy(x => x.Benutzername)
                .ToListAsync();

            return Ok(bestenliste);
        }

        // POST: api/PingPong/spiel
        [HttpPost("spiel")]
        public async Task<IActionResult> SpielSpeichern(
            [FromBody] PingPongSpielErgebnisRequest request)
        {
            if (request.SpielerLinksId <= 0 ||
                request.SpielerRechtsId <= 0 ||
                request.SpielerLinksId == request.SpielerRechtsId)
            {
                return BadRequest(
                    "Es müssen zwei unterschiedliche Benutzer angegeben werden."
                );
            }

            bool gueltigesErgebnis =
                request.ToreLinks >= 0 &&
                request.ToreRechts >= 0 &&
                request.ToreLinks <= 5 &&
                request.ToreRechts <= 5 &&
                request.ToreLinks != request.ToreRechts &&
                (request.ToreLinks == 5 || request.ToreRechts == 5);

            if (!gueltigesErgebnis)
            {
                return BadRequest(
                    "Ein PingPong-Spiel muss mit genau einem Spieler bei 5 Punkten enden."
                );
            }

            List<LoginBenutzer> spieler = await _context.Benutzer
                .Where(x =>
                    x.Id == request.SpielerLinksId ||
                    x.Id == request.SpielerRechtsId)
                .ToListAsync();

            if (spieler.Count != 2)
            {
                return NotFound(
                    "Mindestens einer der angegebenen Benutzer wurde nicht gefunden."
                );
            }

            LoginBenutzer spielerLinks =
                spieler.Single(x => x.Id == request.SpielerLinksId);

            LoginBenutzer spielerRechts =
                spieler.Single(x => x.Id == request.SpielerRechtsId);

            bool linksGewinnt =
                request.ToreLinks > request.ToreRechts;

            LoginBenutzer gewinner =
                linksGewinnt ? spielerLinks : spielerRechts;

            spielerLinks.PingPongToreErzielt += request.ToreLinks;
            spielerLinks.PingPongToreKassiert += request.ToreRechts;

            spielerRechts.PingPongToreErzielt += request.ToreRechts;
            spielerRechts.PingPongToreKassiert += request.ToreLinks;

            gewinner.PingPongSiege++;
            gewinner.PingPongPunkte += 3;

            PingPongSpiel spiel = new PingPongSpiel
            {
                SpielerLinksId = request.SpielerLinksId,
                SpielerRechtsId = request.SpielerRechtsId,
                ToreLinks = request.ToreLinks,
                ToreRechts = request.ToreRechts,
                GewinnerId = gewinner.Id,
                GespieltAm = DateTime.UtcNow
            };

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.PingPongSpiel.Add(spiel);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Ok(new
            {
                spiel.Id,
                GewinnerId = gewinner.Id,
                Gewinner = gewinner.Benutzername,
                Ergebnis = $"{request.ToreLinks}:{request.ToreRechts}"
            });
        }
    }

    public class BestenlisteEintragDto
    {
        public int BenutzerId { get; set; }

        public string Benutzername { get; set; } = string.Empty;

        public int Siege { get; set; }

        public int Punkte { get; set; }

        public int ToreErzielt { get; set; }

        public int ToreKassiert { get; set; }

        public int Torverhaeltnis { get; set; }
    }

    public class PingPongSpielErgebnisRequest
    {
        public int SpielerLinksId { get; set; }

        public int SpielerRechtsId { get; set; }

        public int ToreLinks { get; set; }

        public int ToreRechts { get; set; }
    }
}
