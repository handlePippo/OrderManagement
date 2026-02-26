using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Controllers;

namespace OrderManagement.Order.Api.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly OrderController _sut;
        private readonly IOrderService _client = Substitute.For<IOrderService>();

        public OrderControllerTests()
        {
            _sut = new OrderController(_client);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            _client.ListAsync(default).Returns((IReadOnlyList<OrderDto>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<OrderDto>>();
            ((IReadOnlyList<OrderDto>)ok.Value!).Should().BeEmpty();

            await _client.Received(1).ListAsync(default);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var dto = _fixture.CreateMany<OrderDto>(3).ToArray();
            _client.ListAsync(default).Returns(dto);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).ListAsync(default);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            _client.GetAsync(id, default).Returns((OrderDto?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            await _client.Received(1).GetAsync(id, default);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var dto = _fixture.Create<OrderDto>();
            _client.GetAsync(id, default).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).GetAsync(id, default);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsOkWithBool()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var exists = _fixture.Create<bool>();
            _client.ExistsAsync(id, default).Returns(exists);

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(exists);

            await _client.Received(1).ExistsAsync(id, default);
        }

        [Fact]
        public async Task CreateAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var request = _fixture.Create<CreateOrderDto>();

            // Act
            var result = await _sut.CreateAsync(request, default);

            // Assert
            result.Result.Should().BeOfType<CreatedResult>();
            await _client.Received(1).CreateAsync(request, default);
        }

        [Fact]
        public async Task UpdateAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var request = _fixture.Create<UpdateOrderDto>();

            // Act
            var result = await _sut.UpdateAsync(id, request, default);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _client.Received(1).UpdateAsync(id, request, default);
        }

        [Fact]
        public async Task DeleteAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            // Act
            var result = await _sut.DeleteAsync(id, default);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _client.Received(1).DeleteAsync(id, default);
        }
    }
}