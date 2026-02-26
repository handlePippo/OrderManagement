using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.ValueObjects;
using OrderManagement.Order.Api.Infrastructure.Configuration;
using OrderManagement.Order.Api.Infrastructure.Entities;
using OrderManagement.Order.Api.Infrastructure.Repositories;

namespace OrderManagement.Order.Api.Infrastructure.Tests.Repositories
{
    public sealed class OrderRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();

        private readonly OrderRepository _sut;
        private readonly OrderDbContext _dbContext;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OrderDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);
            _fixture.Inject(_currentUserProvider);

            _sut = _fixture.Create<OrderRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task ListAsync_WhenAdmin_ReturnsAllMappedOrders()
        {
            // Arrange
            _currentUserProvider.IsAdmin.Returns(true);

            _dbContext.Orders.AddRange(
                CreateOrderEntity(userId: 1),
                CreateOrderEntity(userId: 2),
                CreateOrderEntity(userId: 3));
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            var mapped = _fixture.CreateMany<Domain.Entities.Order>(3).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<Domain.Entities.Order>>(Arg.Any<object>()).Returns(mapped);

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

            _dbContext.Orders.AddRange(
                CreateOrderEntity(userId: currentUserId),
                CreateOrderEntity(userId: currentUserId),
                CreateOrderEntity(userId: currentUserId + 1));
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            var mapped = _fixture.CreateMany<Domain.Entities.Order>(2).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<Domain.Entities.Order>>(Arg.Any<object>()).Returns(mapped);

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
            var entity = CreateOrderEntity(userId: 1);
            _dbContext.Orders.Add(entity);
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            // Act
            var result = await _sut.ExistsAsync(entity.Id, default);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_WhenFound_ReturnsMappedOrder()
        {
            // Arrange
            var entity = CreateOrderEntity(userId: 1);
            _dbContext.Orders.Add(entity);
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            var mapped = _fixture.Create<Domain.Entities.Order>();
            _mapper.Map<Domain.Entities.Order>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetAsync(entity.Id, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task AddAsync_AddsMappedEntity()
        {
            // Arrange
            var domain = _fixture.Create<Domain.Entities.Order>();
            var dbEntity = CreateOrderEntity(userId: 1);

            _mapper.Map<OrderEntity>(domain).Returns(dbEntity);

            // Act
            await _sut.AddAsync(domain, default);
            await _dbContext.SaveChangesAsync();

            // Assert
            _mapper.Received(1).Map<OrderEntity>(domain);
            (await _dbContext.Orders.CountAsync(default)).Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteAsync(id, default));
            ex.Message.Should().Be($"Order with id {id} not found.");
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_MapsOntoEntity()
        {
            // Arrange
            var entity = CreateOrderEntity(userId: 1);
            _dbContext.Orders.Add(entity);
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            var domain = new Domain.Entities.Order(entity.Id, entity.UserId, entity.Status);
            domain.SetTotals(3, 2);

            // Act
            await _sut.UpdateAsync(domain, default);
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            // Assert
            _mapper.Received(1).Map(
                Arg.Is<Domain.Entities.Order>(o => o.Id == entity.Id),
                Arg.Is<OrderEntity>(e => e.Id == entity.Id));
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteAsync(id, default));
            ex.Message.Should().Be($"Order with id {id} not found.");
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_RemovesEntity()
        {
            // Arrange
            var entity = CreateOrderEntity(userId: 1);
            _dbContext.Orders.Add(entity);
            await _dbContext.SaveChangesAsync(default);
            _dbContext.ChangeTracker.Clear();

            // Act
            await _sut.DeleteAsync(entity.Id, default);
            await _dbContext.SaveChangesAsync(default);

            // Assert
            (await _dbContext.Orders.AnyAsync(x => x.Id == entity.Id, default)).Should().BeFalse();
        }

        private OrderEntity CreateOrderEntity(int userId) => new(Guid.NewGuid(), userId, OrderStatus.Pending);
    }
}