using SchulApp.Models;
using System.Net.Http.Json;

namespace SchulApp.Services
{
    public class PingPongApiService
    {
        private static readonly HttpClient httpClient = ErstelleHttpClient();

        private static HttpClient ErstelleHttpClient()
        {
            string basisAdresse =
                Environment.GetEnvironmentVariable("SCHULAPP_API_BASE_URL")
                ?? "https://localhost:63635/";

            if (!basisAdresse.EndsWith('/'))
            {
                basisAdresse += "/";
            }

            return new HttpClient
            {
                BaseAddress = new Uri(basisAdresse)
            };
        }

        public async Task<List<BestenlisteEintragModel>> BestenlisteLadenAsync()
        {
            List<BestenlisteEintragModel>? eintraege =
                await httpClient.GetFromJsonAsync<List<BestenlisteEintragModel>>(
                    "api/PingPong/bestenliste"
                );

            return eintraege ?? new List<BestenlisteEintragModel>();
        }

        public async Task SpielSpeichernAsync(
            int spielerLinksId,
            int spielerRechtsId,
            int toreLinks,
            int toreRechts)
        {
            PingPongSpielErgebnisRequest request = new PingPongSpielErgebnisRequest
            {
                SpielerLinksId = spielerLinksId,
                SpielerRechtsId = spielerRechtsId,
                ToreLinks = toreLinks,
                ToreRechts = toreRechts
            };

            using HttpResponseMessage response =
                await httpClient.PostAsJsonAsync(
                    "api/PingPong/spiel",
                    request
                );

            response.EnsureSuccessStatusCode();
        }

        private sealed class PingPongSpielErgebnisRequest
        {
            public int SpielerLinksId { get; set; }

            public int SpielerRechtsId { get; set; }

            public int ToreLinks { get; set; }

            public int ToreRechts { get; set; }
        }
    }
}
