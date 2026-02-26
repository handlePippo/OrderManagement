namespace OrderManagement.Order.Api.Domain.ValueObjects;

public sealed record ProductStockLine
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}