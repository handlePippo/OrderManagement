namespace OrderManagement.Category.Api.Application.DTOs;

public sealed record UpdateCategoryDto
{
    public string? Name { get; init; } = null!;
}