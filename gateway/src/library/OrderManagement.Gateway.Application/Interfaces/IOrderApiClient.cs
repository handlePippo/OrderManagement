using OrderManagement.Gateway.Application.DTOs.Orders;

namespace OrderManagement.Gateway.Application.Interfaces;

public interface IOrderApiClient
{
    Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateOrderDto entity, CancellationToken cancellationToken = default);
    Task SubmitAsync(Guid id, CancellationToken token);
    Task UpdateAsync(Guid id, UpdateOrderDto entity, CancellationToken cancellationToken = default);
    Task DeleteSubmittedAsync(Guid id, CancellationToken token);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}