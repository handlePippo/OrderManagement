using OrderManagement.Product.Api.Application.DTOs;

namespace OrderManagement.Product.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this Domain.Entities.Product product, UpdateProductDto dto)
        {
            ArgumentNullException.ThrowIfNull(product);
            ArgumentNullException.ThrowIfNull(dto);

            var updateCount = 0;
            if (dto.CategoryId is int categoryId && categoryId > 0)
            {
                product.SetCategoryId(categoryId);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.Sku))
            {
                product.SetSku(dto.Sku);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                product.SetName(dto.Name);
                updateCount++;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                product.SetDescription(dto.Description);
                updateCount++;
            }

            if (dto.Price is decimal price)
            {
                product.SetPrice(price);
                updateCount++;
            }

            if (updateCount > 0)
            {
                product.MarkModified();
            }
        }
    }
}