namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record UpdateOrderDto
{
    public int? AddressId { get; set; }
    public IReadOnlyList<OrderItemProductInfoDto>? Items { get; set; }
}