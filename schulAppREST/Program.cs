using Microsoft.EntityFrameworkCore;
using SchulApp.Data;

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

// Controller
builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();