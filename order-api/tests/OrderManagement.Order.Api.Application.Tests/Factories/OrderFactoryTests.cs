using FluentAssertions;
using OrderManagement.Order.Api.Application.Factories;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Tests.Factories
{
    public sealed class OrderFactoryTests
    {
        [Fact]
        public void Create_WhenShippingAddressIsNull_ThrowsArgumentNullException()
        {
            // Act
            var act = () => OrderFactory.Create(
                userId: 10,
                shippingAddress: null!,
                subTotal: 100,
                total: 120);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_SetsAllOrderFieldsCorrectly()
        {
            // Arrange
            var userId = 42;

            var address = new ShippingAddress
            {
                ShipAddress = "Address",
                ShipCity = "City",
                ShipCountryCode = "CC",
                ShipPhoneNumber = "1234567890",
                ShipPostalCode = "12345",
            };

            var subTotal = 100m;
            var total = 120m;

            // Act
            var order = OrderFactory.Create(userId, address, subTotal, total);

            // Assert
            order.Should().NotBeNull();

            order.Id.Should().NotBe(Guid.Empty);
            order.UserId.Should().Be(userId);
            order.Status.Should().Be(OrderStatus.Pending);

            order.ShippingAddress.Should().Be(address);
            order.SubTotal.Should().Be(subTotal);
            order.Total.Should().Be(total);
        }

        [Fact]
        public void Create_GeneratesDifferentIds()
        {
            // Arrange
            var address = new ShippingAddress
            {
                ShipAddress = "Address",
                ShipCity = "City",
                ShipCountryCode = "CC",
                ShipPhoneNumber = "1234567890",
                ShipPostalCode = "12345",
            };

            // Act
            var order1 = OrderFactory.Create(1, address, 10, 12);
            var order2 = OrderFactory.Create(1, address, 10, 12);

            // Assert
            order1.Id.Should().NotBe(order2.Id);
        }
    }
}