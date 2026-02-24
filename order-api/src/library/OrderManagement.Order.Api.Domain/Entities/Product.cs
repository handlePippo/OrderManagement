namespace OrderManagement.Order.Api.Domain.Entities;

public sealed class Product
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string Sku { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
}
