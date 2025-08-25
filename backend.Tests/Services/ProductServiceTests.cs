using FluentAssertions;
using Moq;

using backend.Services;
using backend.Repositories;
using backend.Common.Mapping;
using backend.Models;
using backend.DTOs;
using backend.Exceptions;

namespace backend.Tests.Services
{
    public class ProductServiceTests
    {
        // Use the real reflection-based mapper to avoid brittle mapping mocks
        private readonly IObjectMapper _mapper = new ReflectionObjectMapper();

        [Fact]
        public async Task GetPagedAsync_ReturnsDtos_WhenDataExists()
        {
            // Arrange
            var repo = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product { Name = "Prod A", Description = "Desc A", Price = 10.5m, Stock = 3 },
                new Product { Name = "Prod B", Description = "Desc B", Price = 20.0m, Stock = 7 }
            };
            repo.Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((products, products.Count));

            var sut = new ProductService(repo.Object, _mapper);

            // Act
            var result = await sut.GetPagedAsync(1, 10, CancellationToken.None);

            // Assert
            result.TotalRecords.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items[0].Should().BeEquivalentTo(new ProductResponseDto
            {
                Name = "Prod A",
                Description = "Desc A",
                Price = 10.5m,
                Stock = 3
            });
            result.Items[1].Should().BeEquivalentTo(new ProductResponseDto
            {
                Name = "Prod B",
                Description = "Desc B",
                Price = 20.0m,
                Stock = 7
            });
        }

        [Fact]
        public async Task GetPagedAsync_ThrowsNotFound_WhenNoData()
        {
            // Arrange
            var repo = new Mock<IProductRepository>();
            repo.Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Product>(), 0));

            var sut = new ProductService(repo.Object, _mapper);

            // Act
            var act = () => sut.GetPagedAsync(1, 10, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Product*records*found*");
        }
    }
}
