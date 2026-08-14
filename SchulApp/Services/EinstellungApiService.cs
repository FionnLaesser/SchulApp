using System.Net.Http.Json;

namespace SchulApp.Services
{
    public sealed class EinstellungApiService
    {
        private const int StandardBallSpeed = 6;
        private const int MinBallSpeed = 1;
        private const int MaxBallSpeed = 20;

        private static readonly HttpClient httpClient = HttpClientErstellen();

        public async Task<int> BallSpeedLadenAsync(int benutzerId)
        {
            using HttpResponseMessage response =
                await httpClient.GetAsync(
                    $"api/Einstellung/{benutzerId}/ball-speed"
                );

            response.EnsureSuccessStatusCode();

            BallSpeedAntwort? antwort =
                await response.Content.ReadFromJsonAsync<BallSpeedAntwort>();

            return Math.Clamp(
                antwort?.BallSpeed ?? StandardBallSpeed,
                MinBallSpeed,
                MaxBallSpeed
            );
        }

        public async Task BallSpeedSpeichernAsync(
            int benutzerId,
            int ballSpeed,
            int hintergrundFarbe,
            int textFarbe)
        {
            BallSpeedAnfrage anfrage = new BallSpeedAnfrage
            {
                BallSpeed = Math.Clamp(
                    ballSpeed,
                    MinBallSpeed,
                    MaxBallSpeed
                ),
                HintergrundFarbe = hintergrundFarbe,
                TextFarbe = textFarbe
            };

            using HttpResponseMessage response =
                await httpClient.PutAsJsonAsync(
                    $"api/Einstellung/{benutzerId}/ball-speed",
                    anfrage
                );

            response.EnsureSuccessStatusCode();
        }

        private static HttpClient HttpClientErstellen()
        {
            string? konfigurierteUrl =
                Environment.GetEnvironmentVariable(
                    "SCHULAPP_API_BASE_URL"
                );

            string baseUrl = string.IsNullOrWhiteSpace(konfigurierteUrl)
                ? "https://localhost:63635/"
                : konfigurierteUrl.Trim();

            if (!baseUrl.EndsWith('/'))
            {
                baseUrl += "/";
            }

            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        private sealed class BallSpeedAntwort
        {
            public int BallSpeed { get; set; }
        }

        private sealed class BallSpeedAnfrage
        {
            public int BallSpeed { get; set; }

            public int HintergrundFarbe { get; set; }

            public int TextFarbe { get; set; }
        }
    }
}
