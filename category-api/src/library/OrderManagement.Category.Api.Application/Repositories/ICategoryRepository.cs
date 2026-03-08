using OrderManagement.Category.Api.Domain.Pagination;

namespace OrderManagement.Category.Api.Application.Repositories
{
    public interface ICategoryRepository
    {
        Task<Domain.Entities.Category?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Domain.Entities.Category>> ListAsync(ListRequest pagination, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Entities.Category entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Entities.Category entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}