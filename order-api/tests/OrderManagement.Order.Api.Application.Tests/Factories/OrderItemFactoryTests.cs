using FluentAssertions;
using OrderManagement.Order.Api.Application.Factories;

namespace OrderManagement.Order.Api.Application.Tests.Factories
{
    public sealed class OrderItemFactoryTests
    {
        [Theory]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WhenNameIsInvalid_Throws(string name)
        {
            var act = () => OrderItemFactory.Create(Guid.NewGuid(), 1, name, 10, 1);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WhenProductIdInvalid_Throws()
        {
            var act = () => OrderItemFactory.Create(Guid.NewGuid(), 0, "name", 10, 1);

            act.Should().Throw<ArgumentException>().WithMessage("*product id*");
        }

        [Fact]
        public void Create_WhenPriceInvalid_Throws()
        {
            var act = () => OrderItemFactory.Create(Guid.NewGuid(), 1, "name", 0, 1);

            act.Should().Throw<ArgumentException>().WithMessage("*price*");
        }

        [Fact]
        public void Create_WhenQtyInvalid_Throws()
        {
            var act = () => OrderItemFactory.Create(Guid.NewGuid(), 1, "name", 10, 0);

            act.Should().Throw<ArgumentException>().WithMessage("*quantity*");
        }

        [Fact]
        public void Create_SetsAllFieldsCorrectly()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            var item = OrderItemFactory.Create(orderId, 10, "Keyboard", 99.9m, 2);

            // Assert
            item.OrderId.Should().Be(orderId);
            item.ProductId.Should().Be(10);
            item.ProductName.Should().Be("Keyboard");
            item.UnitPrice.Should().Be(99.9m);
            item.Quantity.Should().Be(2);
        }
    }
}