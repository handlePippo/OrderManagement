namespace OrderManagement.Gateway.Application.DTOs.Products;

public sealed record UpdateProductDto
{
    public int? CategoryId { get; set; } = null;
    public string? Sku { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}