using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using OrderManagement.Provisioner.Api.Infrastructure.Configuration;
using OrderManagement.Provisioner.Api.Infrastructure.Entities;
using OrderManagement.Provisioner.Api.Infrastructure.Repositories;

namespace OrderManagement.Provisioner.Api.Infrastructure.Tests.Repositories
{
    public sealed class UserRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();
        private readonly UserRepository _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly UserDbContext _dbContext;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new UserDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);
            _fixture.Inject(_currentUserProvider);

            _sut = _fixture.Create<UserRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task ListAsync_WhenAdmin_ReturnsAllMappedUsers()
        {
            // Arrange
            _currentUserProvider.IsAdmin.Returns(true);

            var entities = _fixture.CreateMany<UserEntity>(4).ToList();
            _dbContext.Users.AddRange(entities);
            await _dbContext.SaveChangesAsync(default);

            var mapped = _fixture.CreateMany<User>(4).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<User>>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(mapped);
            _currentUserProvider.DidNotReceive().GetLoggedUserId();
        }

        [Fact]
        public async Task ListAsync_WhenNotAdmin_FiltersByCurrentUserId()
        {
            // Arrange
            _currentUserProvider.IsAdmin.Returns(false);

            var currentUserId = _fixture.Create<int>();
            _currentUserProvider.GetLoggedUserId().Returns(currentUserId);

            var me = CreateUserEntityWithId(currentUserId);
            var other = CreateUserEntityWithId(currentUserId + 1);

            _dbContext.Users.AddRange(me, other);
            await _dbContext.SaveChangesAsync(default);

            var mapped = _fixture.CreateMany<User>(1).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<User>>(Arg.Any<object>()).Returns(mapped);

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
            var entity = _fixture.Create<UserEntity>();
            _dbContext.Users.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var id = entity.Id;

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_ByTokenRequest_WhenMatchNotFound_ReturnsNull()
        {
            // Arrange
            var request = _fixture.Create<TokenRequest>();

            _mapper.Map<User>(Arg.Is<UserEntity?>(x => x == null)).Returns((User?)null);

            // Act
            var result = await _sut.GetAsync(request, default);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_ByTokenRequest_WhenMatchFound_MapsAndReturnsUser()
        {
            // Arrange
            var entity = _fixture.Create<UserEntity>();

            _dbContext.Users.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var request = new TokenRequest(entity.Email, entity.PasswordHash);

            var mapped = _fixture.Create<User>();
            _mapper.Map<User>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetAsync(request, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task GetAsync_ById_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var id = 999999;

            _mapper.Map<User>(Arg.Is<UserEntity?>(x => x == null)).Returns((User?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_ById_WhenFound_MapsAndReturnsUser()
        {
            // Arrange
            var entity = _fixture.Create<UserEntity>();
            _dbContext.Users.Add(entity);
            await _dbContext.SaveChangesAsync(default);

            var id = entity.Id;

            var mapped = _fixture.Create<User>();
            _mapper.Map<User>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task AddAsync_AddsMappedEntity_AndPersists()
        {
            // Arrange
            var domain = _fixture.Create<User>();
            var entity = _fixture.Create<UserEntity>();

            _mapper.Map<UserEntity>(domain).Returns(entity);

            // Act
            await _sut.AddAsync(domain, default);

            // Assert
            _mapper.Received(1).Map<UserEntity>(domain);
            (await _dbContext.Users.CountAsync(default)).Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = _fixture.Create<User>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(user, default));
            ex.Message.Should().Be($"User with id {user.Id} not found.");
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_MapsOntoEntity_AndPersists()
        {
            // Arrange
            var existing = _fixture.Create<UserEntity>();
            _dbContext.Users.Add(existing);
            await _dbContext.SaveChangesAsync(default);

            var id = existing.Id;
            var domain = new User(id);

            // Act
            await _sut.UpdateAsync(domain, default);

            // Assert
            (await _dbContext.Users.AnyAsync(x => x.Id == id, default)).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange

            // Act
            var act = () => _sut.DeleteAsync(404, default);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User with id 404 not found.");
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_RemovesEntity_AndPersists()
        {
            // Arrange
            var existing = _fixture.Create<UserEntity>();
            _dbContext.Users.Add(existing);
            await _dbContext.SaveChangesAsync(default);

            var id = existing.Id;

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            (await _dbContext.Users.AnyAsync(x => x.Id == id, default)).Should().BeFalse();
        }

        private UserEntity CreateUserEntityWithId(int id) => new(id, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
    }
}