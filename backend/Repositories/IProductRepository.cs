using backend.Models;

namespace backend.Repositories
{
    /// <summary>
    /// Abstraction for product data access.
    /// </summary>
    public interface IProductRepository
    {
        // Returns paged products and total count.
        Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
