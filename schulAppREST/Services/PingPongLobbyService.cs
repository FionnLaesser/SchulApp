using System.Security.Cryptography;

namespace schulAppREST.Services
{
    public sealed class PingPongLobbyService
    {
        private const int GameCodeLength = 6;
        private const string GameCodeCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private static readonly TimeSpan LobbyLifetime = TimeSpan.FromMinutes(30);

        private readonly object syncRoot = new object();
        private readonly Dictionary<string, PingPongLobbySession> sessions =
            new Dictionary<string, PingPongLobbySession>(StringComparer.OrdinalIgnoreCase);

        public PingPongLobbySession CreateLobby(int hostUserId, string hostUsername)
        {
            lock (syncRoot)
            {
                RemoveExpiredLobbies();

                PingPongLobbySession? existingLobby = sessions.Values
                    .FirstOrDefault(x => x.HostUserId == hostUserId);

                if (existingLobby != null)
                {
                    existingLobby.UpdatedAtUtc = DateTime.UtcNow;
                    return existingLobby.Copy();
                }

                string code = GenerateUniqueGameCode();
                DateTime now = DateTime.UtcNow;

                PingPongLobbySession lobby = new PingPongLobbySession
                {
                    Code = code,
                    HostUserId = hostUserId,
                    HostUsername = hostUsername,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                sessions.Add(code, lobby);
                return lobby.Copy();
            }
        }

        public PingPongLobbyJoinResult JoinLobby(
            string gameCode,
            int userId,
            string username)
        {
            lock (syncRoot)
            {
                RemoveExpiredLobbies();

                if (!sessions.TryGetValue(NormalizeCode(gameCode), out PingPongLobbySession? lobby))
                {
                    return new PingPongLobbyJoinResult(
                        PingPongLobbyJoinStatus.NotFound,
                        null
                    );
                }

                if (lobby.HostUserId == userId)
                {
                    return new PingPongLobbyJoinResult(
                        PingPongLobbyJoinStatus.SamePlayer,
                        lobby.Copy()
                    );
                }

                if (lobby.GuestUserId == userId)
                {
                    lobby.UpdatedAtUtc = DateTime.UtcNow;
                    return new PingPongLobbyJoinResult(
                        PingPongLobbyJoinStatus.Success,
                        lobby.Copy()
                    );
                }

                if (lobby.GuestUserId.HasValue)
                {
                    return new PingPongLobbyJoinResult(
                        PingPongLobbyJoinStatus.Full,
                        lobby.Copy()
                    );
                }

                lobby.GuestUserId = userId;
                lobby.GuestUsername = username;
                lobby.UpdatedAtUtc = DateTime.UtcNow;

                return new PingPongLobbyJoinResult(
                    PingPongLobbyJoinStatus.Success,
                    lobby.Copy()
                );
            }
        }

        public PingPongLobbySession? GetLobby(string gameCode)
        {
            lock (syncRoot)
            {
                RemoveExpiredLobbies();

                if (!sessions.TryGetValue(NormalizeCode(gameCode), out PingPongLobbySession? lobby))
                {
                    return null;
                }

                lobby.UpdatedAtUtc = DateTime.UtcNow;
                return lobby.Copy();
            }
        }

        public PingPongLobbyCloseStatus CloseLobby(string gameCode, int hostUserId)
        {
            lock (syncRoot)
            {
                RemoveExpiredLobbies();

                string normalizedCode = NormalizeCode(gameCode);

                if (!sessions.TryGetValue(normalizedCode, out PingPongLobbySession? lobby))
                {
                    return PingPongLobbyCloseStatus.NotFound;
                }

                if (lobby.HostUserId != hostUserId)
                {
                    return PingPongLobbyCloseStatus.NotHost;
                }

                sessions.Remove(normalizedCode);
                return PingPongLobbyCloseStatus.Success;
            }
        }

        public PingPongLobbyLeaveStatus LeaveLobby(string gameCode, int userId)
        {
            lock (syncRoot)
            {
                RemoveExpiredLobbies();

                string normalizedCode = NormalizeCode(gameCode);

                if (!sessions.TryGetValue(normalizedCode, out PingPongLobbySession? lobby))
                {
                    return PingPongLobbyLeaveStatus.NotFound;
                }

                if (lobby.HostUserId == userId)
                {
                    sessions.Remove(normalizedCode);
                    return PingPongLobbyLeaveStatus.LobbyClosed;
                }

                if (lobby.GuestUserId != userId)
                {
                    return PingPongLobbyLeaveStatus.NotInLobby;
                }

                lobby.GuestUserId = null;
                lobby.GuestUsername = null;
                lobby.UpdatedAtUtc = DateTime.UtcNow;

                return PingPongLobbyLeaveStatus.Success;
            }
        }

        private string GenerateUniqueGameCode()
        {
            string code;

            do
            {
                char[] characters = new char[GameCodeLength];

                for (int i = 0; i < characters.Length; i++)
                {
                    int index = RandomNumberGenerator.GetInt32(GameCodeCharacters.Length);
                    characters[i] = GameCodeCharacters[index];
                }

                code = new string(characters);
            }
            while (sessions.ContainsKey(code));

            return code;
        }

        private void RemoveExpiredLobbies()
        {
            DateTime threshold = DateTime.UtcNow - LobbyLifetime;

            string[] expiredCodes = sessions
                .Where(x => x.Value.UpdatedAtUtc < threshold)
                .Select(x => x.Key)
                .ToArray();

            foreach (string code in expiredCodes)
            {
                sessions.Remove(code);
            }
        }

        private static string NormalizeCode(string gameCode)
        {
            return (gameCode ?? string.Empty).Trim().ToUpperInvariant();
        }
    }

    public sealed class PingPongLobbySession
    {
        public string Code { get; set; } = string.Empty;
        public int HostUserId { get; set; }
        public string HostUsername { get; set; } = string.Empty;
        public int? GuestUserId { get; set; }
        public string? GuestUsername { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public string Status => GuestUserId.HasValue ? "Ready" : "Waiting";

        public PingPongLobbySession Copy()
        {
            return new PingPongLobbySession
            {
                Code = Code,
                HostUserId = HostUserId,
                HostUsername = HostUsername,
                GuestUserId = GuestUserId,
                GuestUsername = GuestUsername,
                CreatedAtUtc = CreatedAtUtc,
                UpdatedAtUtc = UpdatedAtUtc
            };
        }
    }

    public sealed class PingPongLobbyJoinResult
    {
        public PingPongLobbyJoinResult(
            PingPongLobbyJoinStatus status,
            PingPongLobbySession? lobby)
        {
            Status = status;
            Lobby = lobby;
        }

        public PingPongLobbyJoinStatus Status { get; }
        public PingPongLobbySession? Lobby { get; }
    }

    public enum PingPongLobbyJoinStatus
    {
        Success,
        NotFound,
        Full,
        SamePlayer
    }

    public enum PingPongLobbyCloseStatus
    {
        Success,
        NotFound,
        NotHost
    }

    public enum PingPongLobbyLeaveStatus
    {
        Success,
        LobbyClosed,
        NotFound,
        NotInLobby
    }
}
