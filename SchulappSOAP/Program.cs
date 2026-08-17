using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using SchulAppSOAP.Contracts;
using SchulAppSOAP.Data;
using SchulAppSOAP.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionString 'DefaultConnection' wurde nicht gefunden."
    );

builder.Services.AddDbContext<SchulAppContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<SchuelerService>();

builder.Services
    .AddServiceModelServices() // aktiviert CoreWCF Servicefunktionen
    .AddServiceModelMetadata(); // aktiviert Metadaten wie WSDL

var app = builder.Build();

app.UseRouting();

// Requests, Antwortzeiten und HTTP-Statuscodes für Prometheus erfassen.
// Der /metrics-Endpunkt selbst wird ausgeschlossen, damit die Scrapes die Request-Zahlen nicht verfälschen.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/metrics"),
    branch => branch.UseHttpMetrics(options =>
    {
        options.ReduceStatusCodeCardinality();
    })
);

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder
        .AddService<SchuelerService>(options =>
        {
            options.DebugBehavior.IncludeExceptionDetailInFaults = true;
        })
        .AddServiceEndpoint<SchuelerService, ISchuelerService>(
            new BasicHttpBinding(),
            "/SchuelerService.svc"
        );
});

var metadata = app.Services
    .GetRequiredService<ServiceMetadataBehavior>();

metadata.HttpGetEnabled = true;

app.MapMetrics();

app.Run();
