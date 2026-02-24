using AutoMapper;
using OrderManagement.Provisioner.Api.Application.DTOs.Addresses;
using OrderManagement.Provisioner.Api.Application.Extensions;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Domain.Entities;

namespace OrderManagement.Provisioner.Api.Application.Services
{
    /// <summary>
    /// Address service.
    /// </summary>
    public sealed class AddressService : IAddressService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAddressRepository _repository;

        private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="repository"></param>
        public AddressService(IMapper mapper, ICurrentUserProvider currentUserProvider, IAddressRepository repository)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _repository = repository;
        }
        public async Task<IReadOnlyList<AddressDto>> ListAsync(CancellationToken cancellationToken = default)
        {
            var addresses = await _repository.ListAsync(cancellationToken);
            if (addresses is null)
            {
                return null!;
            }

            return _mapper.Map<IReadOnlyList<AddressDto>>(addresses);
        }

        public async Task<AddressDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var address = await _repository.GetAsync(id, cancellationToken);
            if (address is null)
            {
                return null;
            }

            return _mapper.Map<AddressDto>(address);
        }

        public async Task CreateAsync(CreateAddressDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var address = _mapper.Map<Address>(dto);

            address.SetUserId(CurrentUserId);

            await _repository.AddAsync(address, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _repository.ExistsAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(int id, UpdateAddressDto addressDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(addressDto);

            var address = new Address(id, CurrentUserId);
            address.ApplyPatchFrom(addressDto);

            await _repository.UpdateAsync(address, cancellationToken);
        }
    }
}