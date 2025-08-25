using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

using backend.Controllers;
using backend.DTOs;
using backend.Exceptions;
using backend.Services;

namespace backend.Tests.Controllers
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk_WithApiResponseAndMeta()
        {
            // Arrange
            var svc = new Mock<IProductService>();
            var items = new List<ProductResponseDto>
            {
                new ProductResponseDto { Name = "A", Description = "a", Price = 10m, Stock = 1 },
                new ProductResponseDto { Name = "B", Description = "b", Price = 20m, Stock = 2 }
            };
            svc.Setup(s => s.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PagedResult<ProductResponseDto>(items, totalRecords: 2));

            var controller = new ProductController(svc.Object);
            var query = new PaginationParams { Page = 1, PageSize = 10 };

            // Act
            var actionResult = await controller.GetAll(query, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)actionResult.Result!;
            ok.Value.Should().BeOfType<ApiResponse<List<ProductResponseDto>>>();

            var payload = (ApiResponse<List<ProductResponseDto>>)ok.Value!;
            payload.Status.Should().Be("success");
            payload.Data.Should().HaveCount(2);
            payload.Meta.Should().BeOfType<PaginationMeta>();

            var meta = (PaginationMeta)payload.Meta!;
            meta.Page.Should().Be(1);
            meta.PageSize.Should().Be(10);
            meta.TotalRecords.Should().Be(2);
            meta.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_PropagatesNotFoundException_WhenServiceThrows()
        {
            // Arrange
            var svc = new Mock<IProductService>();
            svc.Setup(s => s.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
               .ThrowsAsync(new NotFoundException("Product"));

            var controller = new ProductController(svc.Object);
            var query = new PaginationParams { Page = 1, PageSize = 10 };

            // Act
            var act = async () => await controller.GetAll(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            // Note: In runtime, your global exception middleware will convert this to 404 JSON.
        }
    }
}
