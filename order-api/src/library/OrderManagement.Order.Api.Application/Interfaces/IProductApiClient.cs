using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Interfaces;

public interface IProductApiClient
{
    Task<IReadOnlyList<Product>> GetProductsAsync(ProductRange range, CancellationToken ct);
}
