using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Interfaces;

public interface IProductApiClient
{
    Task<IReadOnlyList<Product>> GetRangeAsync(IReadOnlyList<int> ids, CancellationToken ct);
}