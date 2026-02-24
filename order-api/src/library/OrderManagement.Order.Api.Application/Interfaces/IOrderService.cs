using OrderManagement.Order.Api.Application.DTOs.Orders;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken = default);
        Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(CreateOrderDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, UpdateOrderDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}