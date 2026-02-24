using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;

namespace OrderManagement.Provisioner.Api.Application.Repositories
{
    /// <summary>
    /// Contract for UserRepository.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// List all results (for admin).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a record exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a record by email and password.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User?> GetAsync(TokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a record by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User?> GetAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing record.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing record.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}