using SchulApp.Models;
using System.Net;
using System.Net.Http.Json;

namespace SchulApp.Services
{
    public sealed class LehrerApiService
    {
        private static readonly HttpClient httpClient =
            HttpClientErstellen();

        public async Task<LehrerModel> ErstellenAsync(
            LehrerModel lehrer)
        {
            using HttpRequestMessage request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/Lehrer"
                )
                {
                    Content = JsonContent.Create(lehrer)
                };

            ApiAuditHeaders.Anwenden(request);

            using HttpResponseMessage response =
                await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<LehrerModel>()
                ?? throw new InvalidOperationException(
                    "Die REST API hat keinen Lehrer zurückgegeben."
                );
        }

        public async Task<bool> BearbeitenAsync(
            int lehrerId,
            LehrerModel lehrer)
        {
            using HttpRequestMessage request =
                new HttpRequestMessage(
                    HttpMethod.Put,
                    $"api/Lehrer/{lehrerId}"
                )
                {
                    Content = JsonContent.Create(lehrer)
                };

            ApiAuditHeaders.Anwenden(request);

            using HttpResponseMessage response =
                await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> LoeschenAsync(int lehrerId)
        {
            using HttpRequestMessage request =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"api/Lehrer/{lehrerId}"
                );

            ApiAuditHeaders.Anwenden(request);

            using HttpResponseMessage response =
                await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        private static HttpClient HttpClientErstellen()
        {
            string? konfigurierteUrl =
                Environment.GetEnvironmentVariable(
                    "SCHULAPP_API_BASE_URL"
                );

            string baseUrl =
                string.IsNullOrWhiteSpace(konfigurierteUrl)
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
    }
}
