namespace backend.DTOs
{
    /// <summary>
    /// Metadata for paginated results.
    /// </summary>
    public class PaginationMeta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        public PaginationMeta(int page, int pageSize, int totalRecords)
        {
            Page = page;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
}
