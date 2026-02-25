using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Orders;

public sealed record OrderItemProductInfoDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}
