using Bogus;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Seed
{
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Ensures the database has initial data. Only inserts when the table is empty.
        /// </summary>
        public static async Task SeedAsync(AppDbContext db)
        {
            // Safety check: if the table already has data, do nothing.
            if (await db.Products.AsNoTracking().AnyAsync())
                return;

            // Configure Faker for Product
            var faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(5, 500)))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.CreatedAt, f => DateTime.UtcNow);

            // Generate a realistic batch
            var items = faker.Generate(20);

            // Insert and save
            await db.Products.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Extension method to run the seeding within a scoped service lifetime.
        /// </summary>
        public static async Task SeedDatabaseAsync(this Microsoft.AspNetCore.Builder.WebApplication app)
        {
            // Create a scope to resolve scoped services like AppDbContext
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Optional: you could validate DB connectivity here if needed.
            await SeedAsync(db);
        }
    }
}
