namespace backend.DTOs
{
    /// <summary>
    /// Query parameters for pagination.
    /// </summary>
    public class PaginationParams
    {
        private const int MaxPageSize = 100;

        public int Page { get; set; } = 1;          // 1-based index
        public int PageSize { get; set; } = 10;     // default page size

        // Normalizes values to sane bounds.
        public void Normalize()
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > MaxPageSize) PageSize = MaxPageSize;
        }
    }
}
