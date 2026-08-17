using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
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
    .AddServiceModelServices() //aktiviert CoreWCF Servicefunktionen
    .AddServiceModelMetadata(); //aktiviert Metadaten wie die *WSDL *WSDL (Web Services Description Language) is an XML-based file that tells computer programs how to connect to and use a SOAP web service

var app = builder.Build();

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

app.Run();