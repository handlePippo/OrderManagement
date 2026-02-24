namespace OrderManagement.Order.Api.Persistence.Clients;

public sealed class ApiProduct
{
    public int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
}
