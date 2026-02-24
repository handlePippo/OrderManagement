namespace OrderManagement.Order.Api.Application.Repositories;

public interface IOrderItemRepository
{
    Task AddRangeAsync(IReadOnlyList<Domain.Entities.OrderItem> entities, CancellationToken cancellationToken = default);
    Task DeleteByOrderIdAsync(int id, CancellationToken cancellationToken = default);
}
