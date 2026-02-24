namespace OrderManagement.Product.Api.Infrastructure.Entities;

/// <summary>
/// Product entity.
/// </summary>
public sealed class ProductEntity
{
    public int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private ProductEntity() { } // EF

    public ProductEntity(int categoryId, string sku, string name, decimal price, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku, nameof(sku));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        CategoryId = categoryId;
        Sku = sku;
        Name = name;
        Price = price;
        Description = description;
    }
}