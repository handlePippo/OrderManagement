using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Orders;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using OrderManagement.Gateway.Controllers;

namespace OrderManagement.Gateway.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly UserController _sut;

        private readonly IProvisionerApiUserClient _client = Substitute.For<IProvisionerApiUserClient>();

        public UserControllerTests()
        {
            _sut = new UserController(_client);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            _client.ListAsync(token).Returns((IReadOnlyList<UserDto>?)null!);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<UserDto>>();
            ((IReadOnlyList<UserDto>)ok.Value!).Should().BeEmpty();
            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.Create<IReadOnlyList<UserDto>>();
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
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            _client.GetAsync(id, token).Returns((UserDto?)null);

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
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.Create<UserDto>();
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
            var id = _fixture.Create<int>();
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
        public async Task UpdateAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<UpdateUserDto>();

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
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();

            // Act
            var result = await _sut.DeleteAsync(id, token);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _client.Received(1).DeleteAsync(id, token);
        }
    }
}