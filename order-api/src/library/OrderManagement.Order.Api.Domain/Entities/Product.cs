namespace OrderManagement.Order.Api.Domain.Entities;

public sealed class Product : EntityBase
{
    public int Id { get; set; }
    public int CategoryId { get; init; }
    public string Sku { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public int? Stock { get; init; }
    public decimal Price { get; init; }
}
