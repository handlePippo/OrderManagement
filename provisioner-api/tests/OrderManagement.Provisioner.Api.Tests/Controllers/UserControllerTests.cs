using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Controllers;

namespace OrderManagement.Provisioner.Api.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly UserController _sut;

        private readonly IUserService _service = Substitute.For<IUserService>();

        public UserControllerTests()
        {
            _sut = new UserController(_service);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            _service.ListAsync(default).Returns((IReadOnlyList<UserDto>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<UserDto>>();
            ((IReadOnlyList<UserDto>)ok.Value!).Should().BeEmpty();
            await _service.Received(1).ListAsync(default);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var dto = _fixture.Create<IReadOnlyList<UserDto>>();
            _service.ListAsync(default).Returns(dto);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _service.Received(1).ListAsync(default);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _service.GetAsync(id, default).Returns((UserDto?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            await _service.Received(1).GetAsync(id, default);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UserDto>();
            _service.GetAsync(id, default).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _service.Received(1).GetAsync(id, default);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsOkWithBool()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var exists = _fixture.Create<bool>();
            _service.ExistsAsync(id, default).Returns(exists);

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(exists);

            await _service.Received(1).ExistsAsync(id, default);
        }

        [Fact]
        public async Task CreateAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var request = _fixture.Create<CreateUserDto>();

            // Act
            var result = await _sut.CreateAsync(request, default);

            // Assert
            result.Result.Should().BeOfType<CreatedResult>();

            await _service.Received(1).CreateAsync(request, default);
        }

        [Fact]
        public async Task UpdateAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var request = _fixture.Create<UpdateUserDto>();

            // Act
            var result = await _sut.UpdateAsync(id, request, default);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _service.Received(1).UpdateAsync(id, request, default);
        }

        [Fact]
        public async Task DeleteAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            var result = await _sut.DeleteAsync(id, default);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _service.Received(1).DeleteAsync(id, default);
        }
    }
}