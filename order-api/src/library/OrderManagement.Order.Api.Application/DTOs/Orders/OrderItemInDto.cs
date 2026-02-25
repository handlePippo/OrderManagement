namespace OrderManagement.Order.Api.Application.DTOs.Orders;

public sealed record OrderItemInDto : EntityBaseDto
{
    public Guid OrderId { get; private set; }
    public OrderItemProductInfoDto ProductInfo { get; init; } = null!;

    public OrderItemInDto(int id, Guid orderId)
        : base(id)
    {
        OrderId = orderId;
    }
}
