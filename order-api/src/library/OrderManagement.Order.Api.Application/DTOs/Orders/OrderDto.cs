using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.DTOs.Orders
{
    public sealed record OrderDto
    {
        public Guid Id { get; set; }
        public int UserId { get; init; }
        public OrderStatus Status { get; init; }
        public decimal Subtotal { get; init; }
        public decimal Total { get; init; }
        public ShippingAddressDto ShippingAddress { get; init; } = null!;
        public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>()!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        public OrderDto(Guid id, int userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}