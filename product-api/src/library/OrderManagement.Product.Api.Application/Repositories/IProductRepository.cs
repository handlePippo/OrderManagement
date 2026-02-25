using OrderManagement.Product.Api.Domain.Entities;

namespace OrderManagement.Product.Api.Application.Repositories
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Domain.Entities.Product>> ListAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Product?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Domain.Entities.Product>> GetRangeAsync(GetProductRange ids, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Entities.Product entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Entities.Product entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}