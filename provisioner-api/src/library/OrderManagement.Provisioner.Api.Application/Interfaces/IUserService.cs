using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;

namespace OrderManagement.Provisioner.Api.Application.Interfaces
{
    /// <summary>
    /// Contract for UserService.
    /// </summary>
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default);
        Task<UserDto?> GetAsync(TokenRequestDto request, CancellationToken cancellationToken = default);
        Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task CreateAsync(CreateUserDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateUserDto entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}