namespace OrderManagement.Gateway.Application.DTOs.Products
{
    public sealed record ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}