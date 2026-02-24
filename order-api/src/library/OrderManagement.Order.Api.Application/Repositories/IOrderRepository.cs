namespace OrderManagement.Order.Api.Application.Repositories
{
    public interface IOrderRepository
    {
        Task<IReadOnlyList<Domain.Entities.Order>> ListAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Order?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<int> AddAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}