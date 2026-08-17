using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchulApp.Data;

namespace schulAppREST.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly SchulAppContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HealthController(
            SchulAppContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            bool restApiOk = true;
            bool datenbankOk = false;
            bool soapApiOk = false;

            string? datenbankFehler = null;
            string? soapFehler = null;

            // Datenbank prüfen
            try
            {
                datenbankOk = await _context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                datenbankFehler = ex.Message;
            }

            // SOAP API prüfen
            try
            {
                string? soapHealthUrl =
                    _configuration["SoapApi:HealthUrl"];

                if (!string.IsNullOrWhiteSpace(soapHealthUrl))
                {
                    var client = _httpClientFactory.CreateClient();

                    client.Timeout = TimeSpan.FromSeconds(5);

                    var response = await client.GetAsync(soapHealthUrl);

                    soapApiOk = response.IsSuccessStatusCode;

                    if (!soapApiOk)
                    {
                        soapFehler =
                            $"HTTP {(int)response.StatusCode}";
                    }
                }
                else
                {
                    soapFehler =
                        "SOAP HealthUrl ist nicht konfiguriert.";
                }
            }
            catch (Exception ex)
            {
                soapFehler = ex.Message;
            }

            bool allesOk =
                restApiOk &&
                datenbankOk &&
                soapApiOk;

            var antwort = new
            {
                status = allesOk ? "Healthy" : "Unhealthy",

                timestamp = DateTime.UtcNow,

                components = new
                {
                    restApi = new
                    {
                        status = restApiOk
                            ? "Healthy"
                            : "Unhealthy"
                    },

                    database = new
                    {
                        status = datenbankOk
                            ? "Healthy"
                            : "Unhealthy",

                        error = datenbankFehler
                    },

                    soapApi = new
                    {
                        status = soapApiOk
                            ? "Healthy"
                            : "Unhealthy",

                        error = soapFehler
                    }
                }
            };

            if (allesOk)
            {
                return Ok(antwort);
            }

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                antwort
            );
        }
    }
}