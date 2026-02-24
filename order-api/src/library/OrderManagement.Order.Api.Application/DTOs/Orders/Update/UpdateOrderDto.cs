using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.DTOs.Orders.Update;

public sealed record UpdateOrderDto
{
    public UpdateShippingAddressDto? ShippingAddress { get; init; }
    public IReadOnlyList<OrderItemProductInfoDto>? Items { get; init; }
}
