namespace backend.DTOs
{
    /// <summary>
    /// Internal result carrying items and total count for pagination.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; }
        public int TotalRecords { get; }

        public PagedResult(List<T> items, int totalRecords)
        {
            Items = items;
            TotalRecords = totalRecords;
        }
    }
}
