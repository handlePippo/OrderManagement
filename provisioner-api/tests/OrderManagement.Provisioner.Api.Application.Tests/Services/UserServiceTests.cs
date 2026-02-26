using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Application.Services;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using Xunit;

namespace OrderManagement.Provisioner.Api.Application.Tests.Services
{
    public sealed class UserServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly UserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUserRepository _repository = Substitute.For<IUserRepository>();

        public UserServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_repository);

            _sut = _fixture.Create<UserService>();
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsNull_ReturnsEmptyArray()
        {
            // Arrange
            _repository.ListAsync(default).Returns((IReadOnlyList<User>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mapper.DidNotReceiveWithAnyArgs().Map<IReadOnlyList<UserDto>>(default!);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsUsers_MapsAndReturnsDtos()
        {
            // Arrange
            var users = _fixture.CreateMany<User>(3).ToList().AsReadOnly();
            var dtos = _fixture.CreateMany<UserDto>(3).ToList().AsReadOnly();

            _repository.ListAsync(default).Returns(users);
            _mapper.Map<IReadOnlyList<UserDto>>(users).Returns(dtos);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(dtos);
            await _repository.Received(1).ListAsync(default);
            _mapper.Received(1).Map<IReadOnlyList<UserDto>>(users);
        }

        [Fact]
        public async Task GetAsync_TokenRequest_WhenRequestDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var act = () => _sut.GetAsync((TokenRequestDto)null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().GetAsync(request: default!, default);
        }

        [Fact]
        public async Task GetAsync_TokenRequest_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var requestDto = _fixture.Create<TokenRequestDto>();
            var request = _fixture.Create<TokenRequest>();

            _mapper.Map<TokenRequest>(requestDto).Returns(request);
            _repository.GetAsync(request, default).Returns((User?)null);

            // Act
            var result = await _sut.GetAsync(requestDto, default);

            // Assert
            result.Should().BeNull();
            _mapper.Received(1).Map<TokenRequest>(requestDto);
            await _repository.Received(1).GetAsync(request, default);
            _mapper.DidNotReceiveWithAnyArgs().Map<UserDto>(default!);
        }

        [Fact]
        public async Task GetAsync_TokenRequest_WhenRepositoryReturnsUser_MapsAndReturnsDto()
        {
            // Arrange
            var requestDto = _fixture.Create<TokenRequestDto>();
            var request = _fixture.Create<TokenRequest>();

            var user = _fixture.Create<User>();
            var dto = _fixture.Create<UserDto>();

            _mapper.Map<TokenRequest>(requestDto).Returns(request);
            _repository.GetAsync(request, default).Returns(user);
            _mapper.Map<UserDto>(user).Returns(dto);

            // Act
            var result = await _sut.GetAsync(requestDto, default);

            // Assert
            result.Should().BeSameAs(dto);
            _mapper.Received(1).Map<TokenRequest>(requestDto);
            await _repository.Received(1).GetAsync(request, default);
            _mapper.Received(1).Map<UserDto>(user);
        }

        [Fact]
        public async Task GetAsync_ById_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _repository.GetAsync(id, default).Returns((User?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
            await _repository.Received(1).GetAsync(id, default);
            _mapper.DidNotReceiveWithAnyArgs().Map<UserDto>(default!);
        }

        [Fact]
        public async Task GetAsync_ById_WhenRepositoryReturnsUser_MapsAndReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();

            var user = _fixture.Create<User>();
            var dto = _fixture.Create<UserDto>();

            _repository.GetAsync(id, default).Returns(user);
            _mapper.Map<UserDto>(user).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(dto);
            await _repository.Received(1).GetAsync(id, default);
            _mapper.Received(1).Map<UserDto>(user);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var act = () => _sut.CreateAsync(null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoToEntity_AndAddsToRepository()
        {
            // Arrange
            var dto = _fixture.Create<CreateUserDto>();
            var user = _fixture.Create<User>();

            _mapper.Map<User>(dto).Returns(user);

            // Act
            await _sut.CreateAsync(dto, default);

            // Assert
            _mapper.Received(1).Map<User>(dto);
            await _repository.Received(1).AddAsync(user, default);
        }

        [Fact]
        public async Task DeleteAsync_DelegatesToRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            await _repository.Received(1).DeleteAsync(id, default);
        }

        [Fact]
        public async Task ExistsAsync_DelegatesToRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _repository.ExistsAsync(id, default).Returns(true);

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
            await _repository.Received(1).ExistsAsync(id, default);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            var act = () => _sut.UpdateAsync(id, null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task UpdateAsync_CreatesUserWithId_AndUpdatesRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateUserDto>();

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            await _repository.Received(1).UpdateAsync(
                Arg.Is<User>(u => u.Id == id),
                default);

            // ApplyPatchFrom è extension: non lo verifichi qui senza refactor/test diretto dell’extension.
        }
    }
}