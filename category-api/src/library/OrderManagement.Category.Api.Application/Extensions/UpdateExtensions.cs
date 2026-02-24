using OrderManagement.Category.Api.Application.DTOs;

namespace OrderManagement.Category.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this Domain.Entities.Category category, UpdateCategoryDto dto)
        {
            ArgumentNullException.ThrowIfNull(category);
            ArgumentNullException.ThrowIfNull(dto);

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                category.SetName(dto.Name);
                category.MarkModified();
            }
        }
    }
}