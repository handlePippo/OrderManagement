namespace OrderManagement.Order.Api.Application.Repositories
{
    public interface IOrderRepository
    {
        Task<IReadOnlyList<Domain.Entities.Order>> ListAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Order?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}