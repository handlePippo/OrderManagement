using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Category.Api.Application.DTOs;

public record CreateCategoryDto
{
    [Required]
    public required string Name { get; init; } = null!;
}
