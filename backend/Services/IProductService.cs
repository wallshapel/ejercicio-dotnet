using backend.DTOs;

namespace backend.Services
{
    /// <summary>
    /// Application service for product use-cases.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Returns paginated products mapped to DTOs.
        /// </summary>
        Task<PagedResult<ProductResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
