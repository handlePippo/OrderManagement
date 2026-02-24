using OrderManagement.Product.Api.Application.DTOs;

namespace OrderManagement.Product.Api.Application.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProductDto>> GetRangeAsync(GetProductRangeDto dto, CancellationToken cancellationToken);
        Task AddAsync(CreateProductDto entity, CancellationToken cancellationToken);
        Task UpdateAsync(int id, UpdateProductDto entity, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}