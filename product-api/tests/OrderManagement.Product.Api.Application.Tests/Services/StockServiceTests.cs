using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Application.Repositories;
using OrderManagement.Product.Api.Application.Services;

namespace OrderManagement.Product.Api.Application.Tests.Services
{
    public sealed class StockServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly StockService _sut;
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();

        public StockServiceTests()
        {
            _fixture.Inject(_repository);
            _sut = _fixture.Create<StockService>();
        }

        [Fact]
        public async Task DecreaseStock_WhenRangeNotFound_Throws()
        {
            // Arrange
            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine>()
                {
                    new() { ProductId = _fixture.Create<int>(), Quantity = 1 }
                }
            };

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default).Returns((IReadOnlyList<Domain.Entities.Product>?)null!);

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DecreaseStock(dto, default));

            // Assert
            ex.Message.Should().Be("One or more product not found.");
            await _repository.DidNotReceiveWithAnyArgs().UpdateRangeAsync(default!, default);
        }

        [Fact]
        public async Task DecreaseStock_WhenStockIsZero_Throws_AndDoesNotUpdate()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine>()
                {
                    new() { ProductId = productId, Quantity = 1 }
                }
            };

            var product = CreateProductWithIdAndStock(productId, stock: 0);

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default)
                       .Returns(new List<Domain.Entities.Product> { product });

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DecreaseStock(dto, default));

            // Assert
            ex.Message.Should().Be($"Product {productId} is not avalaible.");
            await _repository.DidNotReceiveWithAnyArgs().UpdateRangeAsync(default!, default);
        }

        [Fact]
        public async Task DecreaseStock_WhenQuantityConsumesAllStock_ClearsStock_AndUpdatesRange()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine>()
                {
                    new() { ProductId = productId, Quantity = 5 }
                }
            };

            var product = CreateProductWithIdAndStock(productId, stock: 5);

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default)
                       .Returns(new List<Domain.Entities.Product> { product });

            // Act
            await _sut.DecreaseStock(dto, default);

            // Assert: repo riceve dict con product aggiornato a stock 0
            await _repository.Received(1).UpdateRangeAsync(
                Arg.Is<Dictionary<int, Domain.Entities.Product>>(d =>
                    d.ContainsKey(productId) && d[productId].Stock == 0),
                default);
        }

        [Fact]
        public async Task DecreaseStock_WhenQuantityLessThanStock_Decreases_AndUpdatesRange()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine> ()
                {
                    new() { ProductId = productId, Quantity = 3 }
                }
            };

            var product = CreateProductWithIdAndStock(productId, stock: 10);

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default)
                       .Returns(new List<Domain.Entities.Product> { product });

            // Act
            await _sut.DecreaseStock(dto, default);

            // Assert
            await _repository.Received(1).UpdateRangeAsync(
                Arg.Is<Dictionary<int, Domain.Entities.Product>>(d =>
                    d.ContainsKey(productId) && d[productId].Stock == 7),
                default);
        }

        [Fact]
        public async Task IncreaseStock_WhenRangeNotFound_Throws()
        {
            // Arrange
            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine>()
                {
                    new() { ProductId = _fixture.Create<int>(), Quantity = 2 }
                }
            };

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default).Returns((IReadOnlyList<Domain.Entities.Product>?)null!);

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.IncreaseStock(dto, default));

            // Assert
            ex.Message.Should().Be("One or more product not found.");
            await _repository.DidNotReceiveWithAnyArgs().UpdateRangeAsync(default!, default);
        }

        [Fact]
        public async Task IncreaseStock_Increases_AndUpdatesRange()
        {
            // Arrange
            var productId = _fixture.Create<int>();

            var dto = new UpdateStockDto
            {
                Lines = new List<StockLine>()
                {
                    new() { ProductId = productId, Quantity = 4 }
                }
            };

            var product = CreateProductWithIdAndStock(productId, stock: 5);

            _repository.GetRangeAsync(Arg.Any<Domain.Entities.ProductRange>(), default)
                       .Returns(new List<Domain.Entities.Product> { product });

            // Act
            await _sut.IncreaseStock(dto, default);

            // Assert
            await _repository.Received(1).UpdateRangeAsync(
                Arg.Is<Dictionary<int, Domain.Entities.Product>>(d =>
                    d.ContainsKey(productId) && d[productId].Stock == 9),
                default);
        }

        private Domain.Entities.Product CreateProductWithIdAndStock(int id, int stock)
        {
            var p = new Domain.Entities.Product(id);
            p.SetStock(stock);
            return p;
        }
    }
}