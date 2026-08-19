using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using schulAppREST.Services;

namespace schulAppREST.Controllers
{
    [ApiController]
    [Route("api/PingPong/lobby")]
    public sealed class PingPongLobbyController : ControllerBase
    {
        private readonly SchulAppContext context;
        private readonly PingPongLobbyService lobbyService;

        public PingPongLobbyController(
            SchulAppContext context,
            PingPongLobbyService lobbyService)
        {
            this.context = context;
            this.lobbyService = lobbyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLobby(
            [FromBody] PingPongLobbyCreateRequest request)
        {
            if (request.HostUserId <= 0)
            {
                return BadRequest("Ungültiger Benutzer.");
            }

            var user = await context.Benutzer
                .AsNoTracking()
                .Where(x => x.Id == request.HostUserId)
                .Select(x => new { x.Id, x.Benutzername })
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Der Benutzer wurde nicht gefunden.");
            }

            PingPongLobbySession lobby =
                lobbyService.CreateLobby(user.Id, user.Benutzername);

            return Ok(lobby);
        }

        [HttpPost("{gameCode}/join")]
        public async Task<IActionResult> JoinLobby(
            string gameCode,
            [FromBody] PingPongLobbyJoinRequest request)
        {
            if (request.UserId <= 0 && string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Ungültiger Benutzer.");
            }

            string username = (request.Username ?? string.Empty).Trim();

            var user = !string.IsNullOrWhiteSpace(username)
                ? await context.Benutzer
                    .AsNoTracking()
                    .Where(x => x.Benutzername == username)
                    .Select(x => new { x.Id, x.Benutzername })
                    .SingleOrDefaultAsync()
                : null;

            if (user == null && request.UserId > 0)
            {
                user = await context.Benutzer
                    .AsNoTracking()
                    .Where(x => x.Id == request.UserId)
                    .Select(x => new { x.Id, x.Benutzername })
                    .SingleOrDefaultAsync();
            }

            if (user == null)
            {
                return NotFound(
                    "Der Benutzer wurde auf dem Host nicht gefunden. Für LAN-Spiele muss der Benutzername in der Host-Datenbank vorhanden sein."
                );
            }

            PingPongLobbyJoinResult result =
                lobbyService.JoinLobby(gameCode, user.Id, user.Benutzername);

            return result.Status switch
            {
                PingPongLobbyJoinStatus.Success => Ok(result.Lobby),
                PingPongLobbyJoinStatus.NotFound => NotFound("Die Lobby wurde nicht gefunden oder ist abgelaufen."),
                PingPongLobbyJoinStatus.Full => Conflict("Die Lobby ist bereits voll."),
                PingPongLobbyJoinStatus.SamePlayer => BadRequest("Der Host kann seiner eigenen Lobby nicht als zweiter Spieler beitreten."),
                _ => BadRequest("Der Lobby konnte nicht beigetreten werden.")
            };
        }

        [HttpGet("{gameCode}")]
        public IActionResult GetLobby(string gameCode)
        {
            PingPongLobbySession? lobby = lobbyService.GetLobby(gameCode);

            if (lobby == null)
            {
                return NotFound("Die Lobby wurde nicht gefunden oder ist abgelaufen.");
            }

            return Ok(lobby);
        }

        [HttpDelete("{gameCode}")]
        public IActionResult CloseLobby(
            string gameCode,
            [FromQuery] int hostUserId)
        {
            PingPongLobbyCloseStatus result =
                lobbyService.CloseLobby(gameCode, hostUserId);

            return result switch
            {
                PingPongLobbyCloseStatus.Success => NoContent(),
                PingPongLobbyCloseStatus.NotFound => NotFound("Die Lobby wurde nicht gefunden."),
                PingPongLobbyCloseStatus.NotHost => StatusCode(StatusCodes.Status403Forbidden, "Nur der Host kann die Lobby schliessen."),
                _ => BadRequest()
            };
        }

        [HttpPost("{gameCode}/leave")]
        public IActionResult LeaveLobby(
            string gameCode,
            [FromBody] PingPongLobbyLeaveRequest request)
        {
            PingPongLobbyLeaveStatus result =
                lobbyService.LeaveLobby(gameCode, request.UserId);

            return result switch
            {
                PingPongLobbyLeaveStatus.Success => NoContent(),
                PingPongLobbyLeaveStatus.LobbyClosed => NoContent(),
                PingPongLobbyLeaveStatus.NotFound => NotFound("Die Lobby wurde nicht gefunden."),
                PingPongLobbyLeaveStatus.NotInLobby => BadRequest("Der Benutzer ist nicht Teil dieser Lobby."),
                _ => BadRequest()
            };
        }
    }

    public sealed class PingPongLobbyCreateRequest
    {
        public int HostUserId { get; set; }
    }

    public sealed class PingPongLobbyJoinRequest
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
    }

    public sealed class PingPongLobbyLeaveRequest
    {
        public int UserId { get; set; }
    }
}
