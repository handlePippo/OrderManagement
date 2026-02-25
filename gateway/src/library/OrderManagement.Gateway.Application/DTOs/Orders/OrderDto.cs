using OrderManagement.Gateway.Domain;

namespace OrderManagement.Gateway.Application.DTOs.Orders
{
    public sealed record OrderDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
        public IReadOnlyList<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>()!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}