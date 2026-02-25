namespace OrderManagement.Gateway.Application.DTOs.Categories;

public sealed record UpdateCategoryDto
{
    public string? Name { get; set; } = null!;
}