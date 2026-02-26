using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Infrastructure.Configuration;
using OrderManagement.Provisioner.Api.Infrastructure.Entities;
using OrderManagement.Provisioner.Api.Infrastructure.Repositories;

namespace OrderManagement.Provisioner.Api.Infrastructure.Tests.Repositories
{
    public sealed class AddressRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();
        private readonly AddressRepository _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly UserDbContext _dbContext;

        public AddressRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new UserDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);
            _fixture.Inject(_currentUserProvider);

            _sut = _fixture.Create<AddressRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task ListAsync_WhenAdmin_ReturnsAllMappedAddresses()
        {
            // Arrange
            _currentUserProvider.IsAdmin.Returns(true);

            var entities = _fixture.CreateMany<AddressEntity>(5).ToList();
            _dbContext.Addresses.AddRange(entities);
            await _dbContext.SaveChangesAsync(default);

            var mapped = _fixture.CreateMany<Address>(5).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<Address>>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(mapped);
            _currentUserProvider.DidNotReceive().GetLoggedUserId();
        }

        [Fact]
        public async Task ListAsync_WhenNotAdmin_FiltersByCurrentUserId_AndReturnsMappedAddresses()
        {
            // Arrange
            _currentUserProvider.IsAdmin.Returns(false);

            var currentUserId = _fixture.Create<int>();
            _currentUserProvider.GetLoggedUserId().Returns(currentUserId);

            var mine1 = CreateAddressEntity(userId: currentUserId);
            var mine2 = CreateAddressEntity(userId: currentUserId);
            var other = CreateAddressEntity(userId: currentUserId + 1);

            _dbContext.Addresses.AddRange(mine1, mine2, other);
            await _dbContext.SaveChangesAsync(default);

            var mapped = _fixture.CreateMany<Address>(2).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<Address>>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(mapped);
            _currentUserProvider.Received(1).GetLoggedUserId();
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var entity = _fixture.Create<AddressEntity>();
            _dbContext.Addresses.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var id = entity.Id;

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WhenNotFound_ReturnsNull()
        {
            // Act
            var result = await _sut.GetAsync(999999, default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<Address>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenFound_ReturnsMappedAddress()
        {
            // Arrange
            var entity = _fixture.Create<AddressEntity>();
            _dbContext.Addresses.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var id = entity.Id;

            var mapped = _fixture.Create<Address>();
            _mapper.Map<Address>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task AddAsync_AddsMappedEntity_AndPersists()
        {
            // Arrange
            var domain = _fixture.Create<Address>();
            var entity = _fixture.Create<AddressEntity>();

            _mapper.Map<AddressEntity>(domain).Returns(entity);

            // Act
            await _sut.AddAsync(domain, default);

            // Assert
            _mapper.Received(1).Map<AddressEntity>(domain);
            (await _dbContext.Addresses.CountAsync(default)).Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var address = _fixture.Create<Address>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(address, default));
            ex.Message.Should().Be($"Address with id {address.Id} not found.");
        }

        [Fact]
        public async Task UpdateAsync_WhenFoundByIdAndUserId_MapsOntoEntity_AndPersists()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            var existing = CreateAddressEntity(userId);
            _dbContext.Addresses.Add(existing);
            await _dbContext.SaveChangesAsync(default);

            var id = existing.Id;

            var domain = new Address(id, userId);

            // Act
            await _sut.UpdateAsync(domain, default);

            // Assert
            _mapper.Received(1).Map(
                Arg.Is<Address>(a => a.Id == id && a.UserId == userId),
                Arg.Is<AddressEntity>(e => e.Id == id && e.UserId == userId));

            (await _dbContext.Addresses.AnyAsync(x => x.Id == id && x.UserId == userId, default))
                .Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange

            // Act
            var act = () => _sut.DeleteAsync(404, default);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Address with id 404 not found.");
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_RemovesEntity_AndPersists()
        {
            // Arrange
            var entity = _fixture.Create<AddressEntity>();
            _dbContext.Addresses.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var id = entity.Id;

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            (await _dbContext.Addresses.AnyAsync(x => x.Id == id, default)).Should().BeFalse();
        }

        private AddressEntity CreateAddressEntity(int userId) => new(userId, "XX", _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
    }
}