namespace OrderManagement.Order.Api.Domain.ValueObjects;

public sealed record ProductStock
{
    public List<ProductStockLine> Lines { get; set; } = new List<ProductStockLine>()!;
}
