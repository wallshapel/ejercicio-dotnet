namespace backend.DTOs
{
    /// <summary>
    /// Generic API response wrapper to standardize successful responses.
    /// </summary>
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success"; // Always "success" for 200 range
        public T Data { get; set; }                     // The actual payload
        public object? Meta { get; set; }               // Optional metadata (e.g., pagination)

        public ApiResponse(T data, object? meta = null)
        {
            Data = data;
            Meta = meta;
        }
    }
}
