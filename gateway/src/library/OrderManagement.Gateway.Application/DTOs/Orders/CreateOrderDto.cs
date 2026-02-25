using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record CreateOrderDto
{
    [Required]
    public required int AddressId { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<OrderItemProductInfoDto> Items { get; init; } = Array.Empty<OrderItemProductInfoDto>()!;
}
