using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Product.Api.Application.DTOs;

public sealed record UpdateProductDto
{
    public int? CategoryId { get; init; } = null;
    public string? Sku { get; init; } = null!;
    public string? Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal? Price { get; init; }
}