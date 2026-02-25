using OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses;

namespace OrderManagement.Gateway.Application.Interfaces.Provisioner;


public interface IProvisionerApiAddressClient
{
    Task<IReadOnlyList<AddressDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<AddressDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateAddressDto entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateAddressDto entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
