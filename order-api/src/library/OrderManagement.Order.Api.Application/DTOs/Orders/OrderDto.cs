using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.DTOs.Orders
{
    public sealed record OrderDto : EntityBaseDto
    {
        public int UserId { get; init; }
        public OrderStatus Status { get; init; }
        public decimal Subtotal { get; init; }
        public decimal Total { get; init; }
        public ShippingAddressDto ShippingAddress { get; init; } = null!;
        public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>()!;

        public OrderDto(int id, int userId) : base(id)
        {
            Status = OrderStatus.Pending;
            UserId = userId;
        }
    }
}