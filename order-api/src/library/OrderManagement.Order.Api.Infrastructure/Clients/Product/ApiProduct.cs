namespace OrderManagement.Order.Api.Infrastructure.Clients.Product;

public sealed class ApiProduct
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
