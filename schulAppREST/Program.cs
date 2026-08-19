using Microsoft.EntityFrameworkCore;
using Prometheus;
using SchulApp.Data;
using SchulAppREST.Exceptions;
using SchulAppREST.Monitoring;
using schulAppREST.Services;

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddSingleton<PingPongLobbyService>();
builder.Services.AddSingleton<PingPongRealtimeService>();

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
app.UseWebSockets();

// Unerwartete API-Fehler zentral behandeln und als einheitliche JSON-Antwort zurückgeben.
app.UseMiddleware<GlobalExceptionMiddleware>();

// /metrics und der Multiplayer-WebSocket müssen über HTTP erreichbar bleiben.
app.UseWhen(
    context =>
        !context.Request.Path.StartsWithSegments("/metrics") &&
        !context.Request.Path.StartsWithSegments("/ws/pingpong"),
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

app.MapGet(
    "/ws/pingpong/{gameCode}",
    async (
        HttpContext context,
        string gameCode,
        int userId,
        PingPongLobbyService lobbyService,
        PingPongRealtimeService realtimeService) =>
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return Results.BadRequest(
                "Für die Multiplayer-Kommunikation ist eine WebSocket-Verbindung erforderlich."
            );
        }

        if (userId <= 0)
        {
            return Results.BadRequest("Eine gültige Benutzer-ID ist erforderlich.");
        }

        PingPongLobbySession? lobby = lobbyService.GetLobby(gameCode);

        if (lobby == null)
        {
            return Results.NotFound("Die PingPong-Lobby wurde nicht gefunden.");
        }

        bool isLobbyMember =
            lobby.HostUserId == userId ||
            lobby.GuestUserId == userId;

        if (!isLobbyMember)
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        using var socket = await context.WebSockets.AcceptWebSocketAsync();

        await realtimeService.HandleConnectionAsync(
            lobby.Code,
            userId,
            lobby.HostUserId,
            socket,
            context.RequestAborted
        );

        return Results.Empty;
    }
);

app.MapMetrics();

app.Run();
