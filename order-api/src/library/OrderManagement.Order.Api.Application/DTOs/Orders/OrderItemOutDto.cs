namespace OrderManagement.Order.Api.Application.DTOs.Orders;

public sealed record OrderItemOutDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = null!;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal => UnitPrice * Quantity;
}
