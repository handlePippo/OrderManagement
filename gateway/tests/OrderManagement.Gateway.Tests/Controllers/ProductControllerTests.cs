using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Products;
using OrderManagement.Gateway.Application.Interfaces;
using OrderManagement.Gateway.Controllers;

namespace OrderManagement.Gateway.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly ProductController _sut;

        private readonly IProductApiClient _client = Substitute.For<IProductApiClient>();

        public ProductControllerTests()
        {
            _sut = new ProductController(_client);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            _client.ListAsync(token).Returns((IReadOnlyList<ProductDto>?)null!);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<ProductDto>>();
            ((IReadOnlyList<ProductDto>)ok.Value!).Should().BeEmpty();

            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.CreateMany<ProductDto>(3).ToArray();
            _client.ListAsync(token).Returns(dto);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).ListAsync(token);
        }

        [Fact]
        public async Task GetRangeAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<GetProductRangeDto>();

            _client.GetRangeAsync(request, token).Returns((IReadOnlyList<ProductDto>?)null);

            // Act
            var result = await _sut.GetRangeAsync(request, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IReadOnlyList<ProductDto>>();
            ((IReadOnlyList<ProductDto>)ok.Value!).Should().BeEmpty();

            await _client.Received(1).GetRangeAsync(request, token);
        }

        [Fact]
        public async Task GetRangeAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<GetProductRangeDto>();
            var dto = _fixture.CreateMany<ProductDto>(4).ToArray();

            _client.GetRangeAsync(request, token).Returns(dto);

            // Act
            var result = await _sut.GetRangeAsync(request, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _client.Received(1).GetRangeAsync(request, token);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            _client.GetAsync(id, token).Returns((ProductDto?)null);

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
            var dto = _fixture.Create<ProductDto>();
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
        public async Task AddAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<CreateProductDto>();

            // Act
            var result = await _sut.AddAsync(request, token);

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
            var request = _fixture.Create<UpdateProductDto>();

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