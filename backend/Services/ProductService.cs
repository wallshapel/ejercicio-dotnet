using backend.Common.Mapping;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    /// <summary>
    /// Implements product use-cases by orchestrating repository and mapping.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IObjectMapper _mapper;

        public ProductService(IProductRepository repo, IObjectMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            // Repository returns items + total count
            var (entities, total) = await _repo.GetPagedAsync(page, pageSize, cancellationToken);

            if (total == 0 || entities.Count == 0)
                throw new NotFoundException("Product");

            var dtos = entities.Select(e => _mapper.Map<Product, ProductResponseDto>(e)).ToList();
            return new PagedResult<ProductResponseDto>(dtos, total);
        }
    }
}
