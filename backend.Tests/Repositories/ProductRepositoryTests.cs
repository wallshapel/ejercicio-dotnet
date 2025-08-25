using backend.Data;
using backend.Models;
using backend.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Repositories;

public class ProductRepositoryTests
{
    private static DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        // Crea opciones únicas para cada test usando el proveedor InMemory
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Asegura una BD nueva por prueba
            .Options;
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageAndTotal()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using var context = new AppDbContext(options);
        context.Products.AddRange(
            new Product { Name = "A", Price = 10, Stock = 5, CreatedAt = DateTime.UtcNow },
            new Product { Name = "B", Price = 20, Stock = 3, CreatedAt = DateTime.UtcNow },
            new Product { Name = "C", Price = 30, Stock = 8, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);

        // Act
        var (items, total) = await repo.GetPagedAsync(page: 1, pageSize: 2);

        // Assert
        total.Should().Be(3);
        items.Should().HaveCount(2);
        items[0].Name.Should().Be("A"); // Verifica orden por nombre
        items[1].Name.Should().Be("B");
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsEmpty_WhenNoProducts()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new AppDbContext(options);

        var repo = new ProductRepository(context);

        // Act
        var (items, total) = await repo.GetPagedAsync(page: 1, pageSize: 5);

        // Assert
        total.Should().Be(0);
        items.Should().BeEmpty();
    }
}
