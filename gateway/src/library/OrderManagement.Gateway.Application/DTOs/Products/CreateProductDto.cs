using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Products;

public record CreateProductDto
{
    [Required]
    public required int CategoryId { get; set; }

    [Required]
    public required string Sku { get; set; } = null!;

    [Required]
    public required string Name { get; set; } = null!;

    [Required]
    public string? Description { get; set; }

    [Required]
    public required decimal Price { get; set; }
}