using OrderManagement.Order.Api.Application.DTOs.Orders;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken);
        Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(CreateOrderDto entity, CancellationToken cancellationToken);
        Task SubmitAsync(Guid id, CancellationToken token);
        Task UpdateAsync(Guid id, UpdateOrderDto entity, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteSubmittedAsync(Guid id, CancellationToken token);
    }
}