namespace OrderManagement.Order.Api.Application.Repositories;

public interface IOrderItemRepository
{
    Task<IReadOnlyList<Domain.Entities.OrderItem>> GetRangeByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.OrderItem>> GetRangeByOrderIdAsync(IReadOnlyList<Guid> orderIds, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IReadOnlyList<Domain.Entities.OrderItem> entities, CancellationToken cancellationToken = default);
    Task DeleteRangeByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
