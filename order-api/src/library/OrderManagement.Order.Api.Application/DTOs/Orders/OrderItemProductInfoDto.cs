using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.DTOs.Orders;

public sealed record OrderItemProductInfoDto
{
    [Required]
    public int ProductId { get; init; }

    [Required]
    public int Quantity { get; init; }
}
