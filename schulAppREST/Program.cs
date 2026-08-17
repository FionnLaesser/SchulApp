using Microsoft.EntityFrameworkCore;
using Prometheus;
using SchulApp.Data;
using SchulAppREST.Monitoring;

var builder = WebApplication.CreateBuilder(args);

// Datenbank
builder.Services.AddDbContext<SchulAppContext>(options =>
    options.UseSqlServer(
        "Server=localhost;" +
        "Database=SchulAppDB;" +
        "Integrated Security=True;" +
        "Encrypt=True;" +
        "TrustServerCertificate=True;"
    )
);

// Datenbankstatus regelmässig als Prometheus-Metrik erfassen
builder.Services.AddHostedService<DatabaseMetricsService>();

// Controller
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchulApp API V1");
    });
}

app.UseRouting();

// /metrics muss über HTTP erreichbar bleiben, damit Prometheus aus Docker scrapen kann.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/metrics"),
    branch => branch.UseHttpsRedirection()
);

// Requests, Antwortzeiten und HTTP-Statuscodes für Prometheus erfassen.
// Der /metrics-Endpunkt selbst wird ausgeschlossen, damit die Scrapes die Request-Zahlen nicht verfälschen.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/metrics"),
    branch => branch.UseHttpMetrics(options =>
    {
        options.ReduceStatusCodeCardinality();
    })
);

app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

app.Run();
