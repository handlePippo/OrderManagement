namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record UpdateOrderDto
{
    public int? AddressId { get; init; }
    public IReadOnlyList<OrderItemProductInfoDto>? Items { get; init; }
}