using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record CreateOrderDto
{
    [Required]
    public required int AddressId { get; set; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<OrderItemProductInfoDto> Items { get; set; } = Array.Empty<OrderItemProductInfoDto>()!;
}
