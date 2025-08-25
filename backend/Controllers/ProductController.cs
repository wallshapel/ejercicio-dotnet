using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns paginated products (DTO). Defaults: page=1, pageSize=10.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetAll([FromQuery] PaginationParams query, CancellationToken ct)
        {
            // Normalize query params
            query.Normalize();

            var paged = await _service.GetPagedAsync(query.Page, query.PageSize, ct);

            var meta = new PaginationMeta(query.Page, query.PageSize, paged.TotalRecords);

            return Ok(new ApiResponse<List<ProductResponseDto>>(paged.Items, meta));
        }

    }
}
