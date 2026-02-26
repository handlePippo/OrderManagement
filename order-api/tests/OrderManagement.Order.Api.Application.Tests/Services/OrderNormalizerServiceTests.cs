using AutoFixture;
using FluentAssertions;
using OrderManagement.Order.Api.Application.Services;
using OrderManagement.Order.Api.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.Tests.Services
{
    public sealed class OrderNormalizerServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly OrderNormalizerService _sut = new();

        [Fact]
        public void NormalizeOrderItems_WhenProductsNull_Throws()
        {
            // Arrange
            var order = new Domain.Entities.Order();
            var productsToBeAdded = new Dictionary<int, int> { [1] = 2 };

            // Act
            var act = () => _sut.NormalizeOrderItems(order, null!, productsToBeAdded);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NormalizeOrderItems_WhenProductsToBeAddedNull_Throws()
        {
            // Arrange
            var order = new Domain.Entities.Order();
            var products = new List<Product> { _fixture.Create<Product>() };

            // Act
            var act = () => _sut.NormalizeOrderItems(order, products, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NormalizeOrderItems_WhenProductIdMissingInDictionary_ThrowsValidationException()
        {
            // Arrange
            var order = new Domain.Entities.Order();
            order.SetId(Guid.NewGuid());

            var p1 = _fixture.Create<Product>();
            var products = new List<Product>
            {
                p1
            };

            var productsToBeAdded = new Dictionary<int, int>
            {
                [2] = 3
            };

            // Act
            var act = () => _sut.NormalizeOrderItems(order, products, productsToBeAdded);

            // Assert
            act.Should().Throw<ValidationException>()
                .WithMessage($"Product {p1.Id} not found.");
        }

        [Fact]
        public void NormalizeOrderItems_CreatesOrderItemsWithCorrectFields()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order();
            order.SetId(orderId);

            var p1 = _fixture.Create<Product>();
            var p2 = _fixture.Create<Product>();
            var products = new List<Product>
            {
                p1,
                p2
            };

            var productsToBeAdded = new Dictionary<int, int>
            {
                [p1.Id] = 2,
                [p2.Id] = 1
            };

            // Act
            var items = _sut.NormalizeOrderItems(order, products, productsToBeAdded);

            // Assert
            items.Should().HaveCount(2);

            items.Should().ContainSingle(i =>
                i.OrderId == orderId &&
                i.ProductId == p1.Id &&
                i.ProductName == p1.Name &&
                i.UnitPrice == p1.Price &&
                i.Quantity == 2);

            items.Should().ContainSingle(i =>
                i.OrderId == orderId &&
                i.ProductId == p2.Id &&
                i.ProductName == p2.Name &&
                i.UnitPrice == p2.Price &&
                i.Quantity == 1);
        }

        [Fact]
        public void NormalizeOrderItems_WhenIsUpdateTrue_DoesNotThrow_AndCreatesItems()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Domain.Entities.Order();
            order.SetId(orderId);

            var p1 = _fixture.Create<Product>();
            var products = new List<Product>
            {
                p1
            };

            var productsToBeAdded = new Dictionary<int, int> { [p1.Id] = 1 };

            // Act
            var items = _sut.NormalizeOrderItems(order, products, productsToBeAdded, isUpdate: true);

            // Assert
            items.Should().HaveCount(1);
        }

        [Fact]
        public void NormalizeOrderItemsCalculations_WhenProductsNull_Throws()
        {
            // Arrange
            var items = new List<OrderItem> { new OrderItem(1, 2) };

            // Act
            var act = () => _sut.NormalizeOrderItemsCalculations(null!, items);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NormalizeOrderItemsCalculations_WhenItemsNull_Throws()
        {
            // Arrange
            var products = new List<Product> { _fixture.Create<Product>() };

            // Act
            var act = () => _sut.NormalizeOrderItemsCalculations(products, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NormalizeOrderItemsCalculations_WhenProductIdMissing_ThrowsValidationException()
        {
            // Arrange
            var products = new List<Product>
            {
                _fixture.Create<Product>()
            };

            var items = new List<OrderItem>
            {
                new OrderItem(productId: 999, quantity: 2)
            };

            // Act
            var act = () => _sut.NormalizeOrderItemsCalculations(products, items);

            // Assert
            act.Should().Throw<ValidationException>()
                .WithMessage("Product 999 not found.");
        }

        [Fact]
        public void NormalizeOrderItemsCalculations_CalculatesSubtotalAndTotal()
        {
            // Arrange
            var p1 = _fixture.Create<Product>();
            var p2 = _fixture.Create<Product>();
            var products = new List<Product>
            {
                p1,
                p2
            };

            var items = new List<OrderItem>
            {
                new OrderItem(productId: p1.Id, quantity: 2), // 2 * 10 = 20
                new OrderItem(productId: p2.Id, quantity: 3)  // 3 * 5 = 15
            };

            var p1Total = p1.Price * 2;
            var p2Total = p2.Price * 3;
            var tot = p1Total + p2Total;

            // Act
            var (subTotal, total, _) = _sut.NormalizeOrderItemsCalculations(products, items);

            // Assert
            subTotal.Should().Be(tot);
            total.Should().Be(tot);
        }
    }
}