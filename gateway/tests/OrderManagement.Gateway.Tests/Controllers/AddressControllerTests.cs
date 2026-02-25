using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using OrderManagement.Gateway.Controllers;

namespace OrderManagement.Gateway.Tests.Controllers
{
    public class AddressControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly AddressController _sut;
        private readonly IProvisionerApiAddressClient _client = Substitute.For<IProvisionerApiAddressClient>();

        public AddressControllerTests()
        {
            _sut = new AddressController(_client);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            _client.ListAsync(token).Returns((IReadOnlyList<AddressDto>?)null!);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            ok.Value.Should().NotBeNull();
            ok.Value.Should().BeAssignableTo<IReadOnlyList<AddressDto>>();

            var list = (IReadOnlyList<AddressDto>)ok.Value!;
            list.Should().NotBeNull();
            list.Should().BeEmpty();

            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.CreateMany<AddressDto>(3).ToArray();
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

            _client.GetAsync(id, token).Returns((AddressDto?)null);

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
            var dto = _fixture.Create<AddressDto>();

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
        public async Task CreateAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<CreateAddressDto>();

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
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<UpdateAddressDto>();

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