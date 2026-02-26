using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Product.Api.Application.Repositories;
using OrderManagement.Product.Api.Application.Services;

namespace OrderManagement.Product.Api.Application.Tests.Services
{
    public sealed class StockServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly StockService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();

        public StockServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_repository);

            _sut = _fixture.Create<StockService>();
        }

        [Fact]
        public async Task DecreaseStock_WhenProductNotFound_Throws()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            var qty = 1;

            _repository.GetAsync(productId, default).Returns((Domain.Entities.Product?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DecreaseStock(productId, qty, default));
            ex.Message.Should().Be($"Product {productId} not found.");

            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task DecreaseStock_WhenStockIsZero_Throws()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            var qty = 1;

            var product = CreateProductWithStock(stock: 0);
            _repository.GetAsync(productId, default).Returns(product);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DecreaseStock(productId, qty, default));
            ex.Message.Should().Be($"Product {productId} is already ended.");

            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task DecreaseStock_WhenQuantityConsumesAllStock_ClearsStock_AndUpdates()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            // stock 5, qty 5 -> stock-qty == 0 => ClearStock + DecreaseStock
            var product = CreateProductWithStock(stock: 5);
            _repository.GetAsync(productId, default).Returns(product);

            // Act
            await _sut.DecreaseStock(productId, quantity: 5, token: default);

            // Assert
            await _repository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Product>(p => p.Stock == 0), default);
        }

        [Fact]
        public async Task DecreaseStock_WhenQuantityLessThanStock_Decreases_AndUpdates()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var product = CreateProductWithStock(stock: 10);
            _repository.GetAsync(productId, default).Returns(product);

            // Act
            await _sut.DecreaseStock(productId, quantity: 3, token: default);

            // Assert
            await _repository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Product>(p => p.Stock == 7), default);
        }

        [Fact]
        public async Task IncreaseStock_WhenProductNotFound_Throws()
        {
            // Arrange
            var productId = _fixture.Create<int>();
            var qty = 2;

            _repository.GetAsync(productId, default).Returns((Domain.Entities.Product?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.IncreaseStock(productId, qty, default));
            ex.Message.Should().Be($"Product {productId} not found.");

            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task IncreaseStock_Increases_AndUpdates()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var product = CreateProductWithStock(stock: 5);
            _repository.GetAsync(productId, default).Returns(product);

            // Act
            await _sut.IncreaseStock(productId, quantity: 4, token: default);

            // Assert
            await _repository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Product>(p => p.Stock == 9), default);
        }

        private Domain.Entities.Product CreateProductWithStock(int stock)
        {
            var p = _fixture.Create<Domain.Entities.Product>();
            p.SetStock(stock);
            return p;
        }
    }
}
