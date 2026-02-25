using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Product.Api.Application.DTOs;

public record GetProductRangeDto
{
    [Required]
    public required IReadOnlyList<int> OrderIds { get; init; }
}