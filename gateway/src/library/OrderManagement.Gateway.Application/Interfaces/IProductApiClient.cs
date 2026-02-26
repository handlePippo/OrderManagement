using OrderManagement.Gateway.Application.DTOs.Products;

namespace OrderManagement.Gateway.Application.Interfaces;

public interface IProductApiClient
{
    Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetRangeAsync(GetProductRangeDto dto, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateProductDto entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateProductDto entity, CancellationToken cancellationToken = default);
    Task IncreaseStock(int productId, int quantity, CancellationToken token);
    Task DecreaseStock(int productId, int quantity, CancellationToken token);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
