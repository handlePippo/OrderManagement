using AutoFixture;
using AutoMapper;
using NSubstitute;
using FluentAssertions;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Application.Services;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Application.DTOs.Addresses;

namespace OrderManagement.Provisioner.Api.Application.Tests.Services
{
    public sealed class AddressServiceTests
    {
        private readonly Fixture _fixture = new();

        private readonly AddressService _sut;

        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly IAddressRepository _repository = Substitute.For<IAddressRepository>();

        public AddressServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_currentUserProvider);
            _fixture.Inject(_repository);

            _sut = _fixture.Create<AddressService>();
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            _repository.ListAsync(default).Returns((IReadOnlyList<Address>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<IReadOnlyList<AddressDto>>(default!);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsAddresses_MapsAndReturnsDtos()
        {
            // Arrange
            var addresses = _fixture.CreateMany<Address>(3).ToList().AsReadOnly();
            var dtos = _fixture.CreateMany<AddressDto>(3).ToList().AsReadOnly();

            _repository.ListAsync(default).Returns(addresses);
            _mapper.Map<IReadOnlyList<AddressDto>>(addresses).Returns(dtos);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(dtos);
            await _repository.Received(1).ListAsync(default);
            _mapper.Received(1).Map<IReadOnlyList<AddressDto>>(addresses);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _repository.GetAsync(id, default).Returns((Address?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<AddressDto>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsAddress_MapsAndReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();

            var address = _fixture.Create<Address>();
            var dto = _fixture.Create<AddressDto>();

            _repository.GetAsync(id, default).Returns(address);
            _mapper.Map<AddressDto>(address).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(dto);
            await _repository.Received(1).GetAsync(id, default);
            _mapper.Received(1).Map<AddressDto>(address);
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
        public async Task CreateAsync_SetsCurrentUserId_ThenAddsToRepository()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            _currentUserProvider.GetLoggedUserId().Returns(userId);

            var dto = _fixture.Create<CreateAddressDto>();

            var entity = _fixture.Create<Address>();
            _mapper.Map<Address>(dto).Returns(entity);

            // Act
            await _sut.CreateAsync(dto, default);

            // Assert
            _mapper.Received(1).Map<Address>(dto);

            await _repository.Received(1).AddAsync(
                Arg.Is<Address>(a => a.UserId == userId),
                default);

            _currentUserProvider.Received(1).GetLoggedUserId();
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
        public async Task UpdateAsync_CreatesAddressWithIdAndCurrentUser_AndUpdatesRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var userId = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateAddressDto>();

            _currentUserProvider.GetLoggedUserId().Returns(userId);

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            await _repository.Received(1).UpdateAsync(
                Arg.Is<Address>(a => a.Id == id && a.UserId == userId),
                default);

            _currentUserProvider.Received(1).GetLoggedUserId();
        }
    }
}