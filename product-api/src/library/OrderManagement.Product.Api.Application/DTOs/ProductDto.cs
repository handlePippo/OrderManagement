namespace OrderManagement.Product.Api.Application.DTOs
{
    public sealed record ProductDto
    {
        public int Id { get; private set; }
        public int CategoryId { get; private set; }
        public string Sku { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
    }
}