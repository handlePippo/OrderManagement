using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Products;

public record GetProductRangeDto
{
    [Required]
    public required IReadOnlyList<int> OrderIds { get; set; }
}