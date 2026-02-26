using OrderManagement.Category.Api.Application.DTOs;

namespace OrderManagement.Category.Api.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken = default);
        Task CreateAsync(CreateCategoryDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateCategoryDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}