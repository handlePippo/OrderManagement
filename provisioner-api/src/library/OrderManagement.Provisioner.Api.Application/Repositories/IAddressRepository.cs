using OrderManagement.Provisioner.Api.Domain.Entities;

namespace OrderManagement.Provisioner.Api.Application.Repositories
{
    /// <summary>
    /// Contract for AddressRepository.
    /// </summary>
    public interface IAddressRepository
    {
        /// <summary>
        /// Lists all results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Address>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a record exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a record by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Address?> GetAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddAsync(Address entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing record.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(Address entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing record.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}