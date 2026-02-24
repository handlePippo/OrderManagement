namespace OrderManagement.Order.Api.Application.DTOs.Orders;

public sealed record OrderItemDto : EntityBaseDto
{
    public int OrderId { get; init; }
    public OrderItemProductInfoDto ProductInfo { get; init; } = null!;

    public OrderItemDto(int id, int orderId)
        : base(id)
    {
        OrderId = orderId;
    }
}
