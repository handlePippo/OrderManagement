using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Categories;

public record CreateCategoryDto
{
    [Required]
    public required string Name { get; set; } = null!;
}
