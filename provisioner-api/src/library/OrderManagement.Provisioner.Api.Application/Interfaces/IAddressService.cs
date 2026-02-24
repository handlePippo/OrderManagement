using OrderManagement.Provisioner.Api.Application.DTOs.Addresses;

namespace OrderManagement.Provisioner.Api.Application.Interfaces
{
    /// <summary>
    /// Contract for AddressService.
    /// </summary>
    public interface IAddressService
    {
        Task<IReadOnlyList<AddressDto>> ListAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<AddressDto?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task CreateAsync(CreateAddressDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateAddressDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}