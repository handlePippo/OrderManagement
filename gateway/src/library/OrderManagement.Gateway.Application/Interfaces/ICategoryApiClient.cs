using OrderManagement.Gateway.Application.DTOs.Categories;

namespace OrderManagement.Gateway.Application.Interfaces;

public interface ICategoryApiClient
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateCategoryDto entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateCategoryDto entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
