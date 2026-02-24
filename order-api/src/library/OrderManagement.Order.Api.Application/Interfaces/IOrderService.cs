using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Application.DTOs.Orders.Update;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken = default);
        Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task CreateAsync(CreateOrderDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateOrderDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}