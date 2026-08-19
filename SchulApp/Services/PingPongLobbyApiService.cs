using SchulApp.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;

namespace SchulApp.Services
{
    public sealed class PingPongLobbyApiService
    {
        private static readonly ConcurrentDictionary<string, HttpClient> Clients =
            new(StringComparer.OrdinalIgnoreCase);

        public async Task<PingPongLobbyModel> CreateLobbyAsync(int hostUserId)
        {
            HttpClient httpClient = GetHttpClient();

            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "api/PingPong/lobby",
                new PingPongLobbyCreateRequest { HostUserId = hostUserId }
            );

            return await ReadRequiredLobbyAsync(response);
        }

        public async Task<PingPongLobbyModel> JoinLobbyAsync(
            string gameCode,
            int userId,
            string? username = null)
        {
            HttpClient httpClient = GetHttpClient();
            string code = EncodeCode(gameCode);

            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                $"api/PingPong/lobby/{code}/join",
                new PingPongLobbyUserRequest
                {
                    UserId = userId,
                    Username = username
                }
            );

            return await ReadRequiredLobbyAsync(response);
        }

        public async Task<PingPongLobbyModel?> GetLobbyAsync(string gameCode)
        {
            HttpClient httpClient = GetHttpClient();
            string code = EncodeCode(gameCode);

            using HttpResponseMessage response =
                await httpClient.GetAsync($"api/PingPong/lobby/{code}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            await EnsureSuccessAsync(response);

            return await response.Content.ReadFromJsonAsync<PingPongLobbyModel>();
        }

        public async Task CloseLobbyAsync(string gameCode, int hostUserId)
        {
            HttpClient httpClient = GetHttpClient();
            string code = EncodeCode(gameCode);

            using HttpResponseMessage response = await httpClient.DeleteAsync(
                $"api/PingPong/lobby/{code}?hostUserId={hostUserId}"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            await EnsureSuccessAsync(response);
        }

        public async Task LeaveLobbyAsync(string gameCode, int userId)
        {
            HttpClient httpClient = GetHttpClient();
            string code = EncodeCode(gameCode);

            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                $"api/PingPong/lobby/{code}/leave",
                new PingPongLobbyUserRequest { UserId = userId }
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            await EnsureSuccessAsync(response);
        }

        private static HttpClient GetHttpClient()
        {
            string baseAddress = PingPongMultiplayerEndpoint.CurrentBaseAddress;

            return Clients.GetOrAdd(
                baseAddress,
                address => new HttpClient
                {
                    BaseAddress = new Uri(address, UriKind.Absolute),
                    Timeout = TimeSpan.FromSeconds(
                        PingPongMultiplayerError.ConnectionTimeoutSeconds
                    )
                }
            );
        }

        private static async Task<PingPongLobbyModel> ReadRequiredLobbyAsync(
            HttpResponseMessage response)
        {
            await EnsureSuccessAsync(response);

            PingPongLobbyModel? lobby =
                await response.Content.ReadFromJsonAsync<PingPongLobbyModel>();

            if (lobby == null || string.IsNullOrWhiteSpace(lobby.Code))
            {
                throw new InvalidOperationException(
                    "Die REST API hat keine gültige Lobby zurückgegeben."
                );
            }

            return lobby;
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            string message = await response.Content.ReadAsStringAsync();
            message = message.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"Lobby-Anfrage fehlgeschlagen ({(int)response.StatusCode}).";
            }

            throw new InvalidOperationException(message);
        }

        private static string EncodeCode(string gameCode)
        {
            string normalized = (gameCode ?? string.Empty)
                .Trim()
                .ToUpperInvariant();

            return Uri.EscapeDataString(normalized);
        }

        private sealed class PingPongLobbyCreateRequest
        {
            public int HostUserId { get; set; }
        }

        private sealed class PingPongLobbyUserRequest
        {
            public int UserId { get; set; }
            public string? Username { get; set; }
        }
    }
}
