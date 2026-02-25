using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record OrderItemProductInfoDto
{
    [Required]
    public int ProductId { get; init; }

    [Required]
    public int Quantity { get; init; }
}
