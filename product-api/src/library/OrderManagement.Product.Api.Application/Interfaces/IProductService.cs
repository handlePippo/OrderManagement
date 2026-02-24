using OrderManagement.Product.Api.Application.DTOs;

namespace OrderManagement.Product.Api.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken cancellationToken = default);
        Task AddAsync(CreateProductDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateProductDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}