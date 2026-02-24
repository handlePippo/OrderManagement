using AutoMapper;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Application.Extensions;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;

namespace OrderManagement.Provisioner.Api.Application.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public sealed class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public UserService(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default)
        {
            var users = await _repository.ListAsync(cancellationToken);
            if (users is null)
            {
                return Array.Empty<UserDto>()!;
            }

            return _mapper.Map<IReadOnlyList<UserDto>>(users);
        }

        public async Task<UserDto?> GetAsync(TokenRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestDto);

            var request = _mapper.Map<TokenRequest>(requestDto);

            var user = await _repository.GetAsync(request, cancellationToken);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetAsync(id, cancellationToken);
            if (user is null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = _mapper.Map<User>(dto);

            await _repository.AddAsync(user, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _repository.ExistsAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(int id, UpdateUserDto userDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            var user = new User(id);
            user.ApplyPatchFrom(userDto);

            await _repository.UpdateAsync(user, cancellationToken);
        }
    }
}