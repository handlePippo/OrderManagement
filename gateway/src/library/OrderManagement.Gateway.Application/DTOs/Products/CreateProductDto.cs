using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Products;

public record CreateProductDto
{
    [Required]
    public required int CategoryId { get; init; }

    [Required]
    public required string Sku { get; init; } = null!;

    [Required]
    public required string Name { get; init; } = null!;

    [Required]
    public string? Description { get; init; }

    [Required]
    public required decimal Price { get; init; }
}