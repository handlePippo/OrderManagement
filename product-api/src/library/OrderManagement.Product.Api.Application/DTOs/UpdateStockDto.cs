using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Product.Api.Application.DTOs;

public sealed record UpdateStockDto
{
    [Required]
    public required IReadOnlyList<StockLine> Lines { get; init; }
}
