namespace OrderManagement.Order.Api.Application.DTOs.Orders;

public sealed record OrderItemDto : EntityBaseDto
{
    public Guid OrderId { get; private set; }
    public OrderItemProductInfoDto ProductInfo { get; init; } = null!;

    public OrderItemDto(int id, Guid orderId)
        : base(id)
    {
        OrderId = orderId;
    }
}
