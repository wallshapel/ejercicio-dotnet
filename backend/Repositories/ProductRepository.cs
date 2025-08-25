using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    /// <summary>
    /// EF Core implementation of IProductRepository.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            // Read-only count first
            var total = await _db.Products.AsNoTracking().CountAsync(cancellationToken);

            // Paged slice
            var items = await _db.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
}
