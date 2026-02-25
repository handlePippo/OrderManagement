namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
