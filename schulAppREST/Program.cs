using Microsoft.EntityFrameworkCore;
using Prometheus;
using SchulApp.Data;
using SchulAppREST.Exceptions;
using SchulAppREST.Monitoring;
using schulAppREST.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SchulAppContext>(options =>
    options.UseSqlServer(
        "Server=localhost;Database=SchulAppDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
    )
);

builder.Services.AddHostedService<DatabaseMetricsService>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditLogService>();
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
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/metrics"),
    branch => branch.UseHttpsRedirection()
);
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
