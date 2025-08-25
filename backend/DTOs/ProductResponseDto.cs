namespace backend.DTOs
{
    /// <summary>
    /// DTO for returning product data in API responses.
    /// </summary>
    public class ProductResponseDto
    {
        public string Name { get; set; } = null!;    // Product name

        public string? Description { get; set; }     // Product description

        public decimal Price { get; set; }           // Product price

        public int Stock { get; set; }               // Available stock
    }
}
