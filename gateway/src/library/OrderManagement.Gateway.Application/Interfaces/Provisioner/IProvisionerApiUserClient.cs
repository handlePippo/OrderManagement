using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;

namespace OrderManagement.Gateway.Application.Interfaces.Provisioner;

public interface IProvisionerApiUserClient
{
    Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateUserDto entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}