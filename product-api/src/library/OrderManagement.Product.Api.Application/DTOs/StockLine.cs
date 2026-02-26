using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Product.Api.Application.DTOs;

public sealed record StockLine
{
    [Required]
    public required int ProductId { get; init; }

    [Required]
    public required int Quantity { get; init; }
}