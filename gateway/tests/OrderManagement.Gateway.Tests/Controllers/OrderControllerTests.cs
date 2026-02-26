using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Orders;
using OrderManagement.Gateway.Application.Interfaces;
using OrderManagement.Gateway.Controllers;

namespace OrderManagement.Gateway.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly OrderController _sut;

        private readonly IOrderApiClient _client = Substitute.For<IOrderApiClient>();

        public OrderControllerTests()
        {
            _sut = new OrderController(_client);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            _client.ListAsync(token).Returns((IReadOnlyList<OrderDto>?)null!);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<OrderDto>>();
            ((IReadOnlyList<OrderDto>)ok.Value!).Should().BeEmpty();

            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.CreateMany<OrderDto>(3).ToArray();
            _client.ListAsync(token).Returns(dto);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var token = _fixture.Create<CancellationToken>();
            _client.GetAsync(id, token).Returns((OrderDto?)null);

            // Act
            var result = await _sut.GetAsync(id, token);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            await _client.Received(1).GetAsync(id, token);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.Create<OrderDto>();
            _client.GetAsync(id, token).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).GetAsync(id, token);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsOkWithBool()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var token = _fixture.Create<CancellationToken>();
            var exists = _fixture.Create<bool>();
            _client.ExistsAsync(id, token).Returns(exists);

            // Act
            var result = await _sut.ExistsAsync(id, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(exists);

            await _client.Received(1).ExistsAsync(id, token);
        }

        [Fact]
        public async Task CreateAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<CreateOrderDto>();

            // Act
            var result = await _sut.CreateAsync(request, token);

            // Assert
            result.Result.Should().BeOfType<CreatedResult>();
            await _client.Received(1).CreateAsync(request, token);
        }

        [Fact]
        public async Task UpdateAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<UpdateOrderDto>();

            // Act
            var result = await _sut.UpdateAsync(id, request, token);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _client.Received(1).UpdateAsync(id, request, token);
        }

        [Fact]
        public async Task DeleteAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var token = _fixture.Create<CancellationToken>();

            // Act
            var result = await _sut.DeleteAsync(id, token);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _client.Received(1).DeleteAsync(id, token);
        }
    }
}