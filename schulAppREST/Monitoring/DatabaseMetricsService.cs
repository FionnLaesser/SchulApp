using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using SchulApp.Data;

namespace SchulAppREST.Monitoring;

public sealed class DatabaseMetricsService : BackgroundService
{
    private static readonly Gauge DatabaseUp = Metrics.CreateGauge(
        "schulapp_database_up",
        "1 wenn die SchulApp-Datenbank erreichbar ist, sonst 0."
    );

    private static readonly Gauge DatabaseCheckDuration = Metrics.CreateGauge(
        "schulapp_database_check_duration_seconds",
        "Dauer des letzten Datenbank-Verbindungschecks in Sekunden."
    );

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseMetricsService> _logger;

    public DatabaseMetricsService(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseMetricsService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SchulAppContext>();

                var canConnect = await dbContext.Database.CanConnectAsync(stoppingToken);
                DatabaseUp.Set(canConnect ? 1 : 0);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                DatabaseUp.Set(0);
                _logger.LogWarning(ex, "Datenbank-Monitoring konnte keine Verbindung herstellen.");
            }
            finally
            {
                stopwatch.Stop();
                DatabaseCheckDuration.Set(stopwatch.Elapsed.TotalSeconds);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
