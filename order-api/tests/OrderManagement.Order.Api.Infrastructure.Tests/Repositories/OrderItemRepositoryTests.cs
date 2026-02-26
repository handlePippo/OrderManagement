using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Infrastructure.Configuration;
using OrderManagement.Order.Api.Infrastructure.Entities;
using OrderManagement.Order.Api.Infrastructure.Repositories;

namespace OrderManagement.Order.Api.Infrastructure.Tests.Repositories
{
    public sealed class OrderItemRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();

        private readonly OrderDbContext _dbContext;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly OrderItemRepository _sut;

        public OrderItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OrderDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);

            _sut = _fixture.Create<OrderItemRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task AddRangeAsync_MapsAndAddsEntities()
        {
            // Arrange
            var domainItems = _fixture.CreateMany<OrderItem>(2).ToList().AsReadOnly();

            var dbItems = new List<OrderItemEntity>
            {
                _fixture.Create<OrderItemEntity>(),
                _fixture.Create<OrderItemEntity>()
            }.AsReadOnly();

            _mapper.Map<IReadOnlyList<OrderItemEntity>>(domainItems).Returns(dbItems);

            // Act
            await _sut.AddRangeAsync(domainItems, default);
            await _dbContext.SaveChangesAsync();

            // Assert
            _mapper.Received(1).Map<IReadOnlyList<OrderItemEntity>>(domainItems);
            (await _dbContext.OrderItems.CountAsync(default)).Should().Be(2);
        }

        [Fact]
        public async Task DeleteRangeByOrderIdAsync_RemovesOnlyItemsForThatOrder()
        {
            // Arrange
            var e1 = CreateOrderItemEntity();
            var e2 = CreateOrderItemEntity();
            var e3 = CreateOrderItemEntity();

            _dbContext.OrderItems.AddRange(e1, e2, e3);
            await _dbContext.SaveChangesAsync(default);

            // Act
            await _sut.DeleteRangeByOrderIdAsync(e1.OrderId, default);
            await _dbContext.SaveChangesAsync(default);

            // Assert
            (await _dbContext.OrderItems.CountAsync(default)).Should().Be(2);
            (await _dbContext.OrderItems.AnyAsync(x => x.OrderId == e1.OrderId, default)).Should().BeFalse();
            (await _dbContext.OrderItems.AnyAsync(x => x.OrderId == e2.OrderId, default)).Should().BeTrue();
            (await _dbContext.OrderItems.AnyAsync(x => x.OrderId == e3.OrderId, default)).Should().BeTrue();
        }

        [Fact]
        public async Task GetRangeByOrderIdAsync_BySingleId_FiltersAndMaps()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();

            var e1 = CreateOrderItemEntity();
            var e2 = CreateOrderItemEntity();
            var e3 = CreateOrderItemEntity();

            _dbContext.OrderItems.AddRange(e1, e2, e3);
            await _dbContext.SaveChangesAsync(default);

            var mapped = _fixture.CreateMany<OrderItem>(2).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<OrderItem>>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetRangeByOrderIdAsync(orderId1, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task GetRangeByOrderIdAsync_ByManyIds_FiltersAndMaps()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var orderId3 = Guid.NewGuid();

            _dbContext.OrderItems.AddRange(
                CreateOrderItemEntity(),
                CreateOrderItemEntity(),
                CreateOrderItemEntity()
            );
            await _dbContext.SaveChangesAsync(default);

            var orderIds = new List<Guid> { orderId1, orderId3 }.AsReadOnly();

            var mapped = _fixture.CreateMany<OrderItem>(2).ToList().AsReadOnly();
            _mapper.Map<IReadOnlyList<OrderItem>>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetRangeByOrderIdAsync(orderIds, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        private OrderItemEntity CreateOrderItemEntity()
        {
            var entity = new OrderItemEntity(_fixture.Create<int>(), _fixture.Create<Guid>(), _fixture.Create<int>());
            entity.ProductName = _fixture.Create<string>();
            entity.Quantity = _fixture.Create<int>();
            return entity;
        }
    }
}