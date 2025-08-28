using backend.Common.Mapping;
using backend.Data;
using backend.Data.Seed;
using backend.Middlewares;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext + retries (handles races/transients when starting containers)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)
    ));

// Services
builder.Services.AddSingleton<IObjectMapper, ReflectionObjectMapper>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handling middleware
app.UseGlobalExceptionHandling();

// === Migrations and seed on startup ===
// Let us ensure that if the DB does not exist/local is empty, it is created, migrated, and seeded.
// This runs both in the container and on your local machine.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
await app.SeedDatabaseAsync();

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Conditional HTTPS: within the container, we prefer simple HTTP to avoid certificates.
// Pass it with the variable DisableHttpsRedirect=true in docker-compose.
var disableHttps = app.Configuration.GetValue<bool>("DisableHttpsRedirect");
if (!disableHttps)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
