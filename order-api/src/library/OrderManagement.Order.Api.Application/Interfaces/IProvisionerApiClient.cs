using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Interfaces;

public interface IProvisionerApiClient
{
    Task<ShippingAddress> GetShippingAddressAsync(int id, CancellationToken ct);
    Task<User> GetUserAsync(int id, CancellationToken ct);
}