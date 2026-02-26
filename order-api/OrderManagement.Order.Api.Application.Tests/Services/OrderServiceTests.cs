using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Application.Services;

namespace OrderManagement.Order.Api.Application.Tests.Services
{
    public sealed class OrderServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly OrderService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IOrderItemRepository _orderItemRepository = Substitute.For<IOrderItemRepository>();
        private readonly IOrderNormalizerService _normalizer = Substitute.For<IOrderNormalizerService>();
        private readonly IProductApiClient _productApiClient = Substitute.For<IProductApiClient>();
        private readonly IProvisionerApiClient _provisionerApiClient = Substitute.For<IProvisionerApiClient>();

        public OrderServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_currentUserProvider);
            _fixture.Inject(_orderRepository);
            _fixture.Inject(_unitOfWork);
            _fixture.Inject(_orderItemRepository);
            _fixture.Inject(_productApiClient);
            _fixture.Inject(_provisionerApiClient);
            _fixture.Inject(_normalizer);

            _sut = _fixture.Create<OrderService>();
        }

        [Fact]
        public async Task ExistsAsync_DelegatesToRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            _orderRepository.ExistsAsync(id, default).Returns(true);

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
            await _orderRepository.Received(1).ExistsAsync(id, default);
        }

        [Fact]
        public async Task ListAsync_WhenOrdersNull_ReturnsEmptyArray()
        {
            // Arrange
            _orderRepository.ListAsync(default).Returns((IReadOnlyList<Domain.Entities.Order>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            await _orderItemRepository.DidNotReceiveWithAnyArgs().GetRangeByOrderIdAsync(orderIds: default!, default);
        }

        [Fact]
        public async Task ListAsync_WhenOrderItemsNull_ReturnsEmptyArray()
        {
            // Arrange
            var orders = _fixture.CreateMany<OrderManagement.Order.Api.Domain.Entities.Order>(2).ToList().AsReadOnly();

            _orderRepository.ListAsync(default).Returns(orders);
            _orderItemRepository.GetRangeByOrderIdAsync(Arg.Any<IReadOnlyList<Guid>>(), default).Returns((IReadOnlyList<Domain.Entities.OrderItem>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAsync_WhenOrderNotFound_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            _orderRepository.GetAsync(id, default).Returns((Domain.Entities.Order?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
            await _orderItemRepository.DidNotReceiveWithAnyArgs().GetRangeByOrderIdAsync(default(Guid), default);
        }

        [Fact]
        public async Task GetAsync_WhenOrderItemsNull_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var order = _fixture.Create<Domain.Entities.Order>();

            _orderRepository.GetAsync(id, default).Returns(order);
            _orderItemRepository.GetRangeByOrderIdAsync(id, default).Returns((IReadOnlyList<Domain.Entities.OrderItem>?)null!);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WhenDtoNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null!, default));
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(Guid.NewGuid(), null!, default));
        }

        [Fact]
        public async Task UpdateAsync_WhenOrderNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = _fixture.Create<UpdateOrderDto>();

            _orderRepository.GetAsync(id, default).Returns((Domain.Entities.Order?)null);

            // Act
            var act = () => _sut.UpdateAsync(id, dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(id, dto, default));
            ex.Message.Should().Be("The requested order does not exists.");
        }

        [Fact]
        public async Task SubmitAsync_WhenOrderNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _orderRepository.GetAsync(id, default).Returns((Domain.Entities.Order?)null);

            // Act &  Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.SubmitAsync(id, default));
            ex.Message.Should().Be("The requested order does not exists.");
        }

        [Fact]
        public async Task DeleteAsync_WhenOrderNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _orderRepository.GetAsync(id, default).Returns((Domain.Entities.Order?)null);

            // Act &  Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteAsync(id, default));
            ex.Message.Should().Be("The requested order does not exists.");
        }
    }
}