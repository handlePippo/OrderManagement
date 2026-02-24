using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.DTOs.Orders.Create;

public sealed record CreateOrderDto
{
    [Required]  
    public required CreateShippingAddressDto ShippingAddress { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<OrderItemProductInfoDto> Items { get; init; } = Array.Empty<OrderItemProductInfoDto>()!;
}
