using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Category.Api.Application.DTOs;
using OrderManagement.Category.Api.Application.Interfaces;
using OrderManagement.Category.Api.Controllers;

namespace OrderManagement.Category.Api.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly CategoryController _sut;

        private readonly ICategoryService _service = Substitute.For<ICategoryService>();

        public CategoryControllerTests()
        {
            _sut = new CategoryController(_service);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsNull_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var pagination = _fixture.Create<ListRequestDto>();

            _service.ListAsync(pagination, default).Returns([]);

            // Act
            var result = await _sut.ListAsync(pagination, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;

            ok.Value.Should().BeAssignableTo<IReadOnlyList<CategoryDto>>();
            ((IReadOnlyList<CategoryDto>)ok.Value!).Should().BeEmpty();

            await _service.Received(1).ListAsync(pagination, default);
        }

        [Fact]
        public async Task ListAsync_WhenClientReturnsList_ReturnsOkWithSameList()
        {
            // Arrange
            var pagination = _fixture.Create<ListRequestDto>();

            var dto = _fixture.CreateMany<CategoryDto>(3).ToArray();
            _service.ListAsync(pagination, default).Returns(dto);

            // Act
            var result = await _sut.ListAsync(pagination, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _service.Received(1).ListAsync(pagination, default);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            _service.GetAsync(id, token).Returns((CategoryDto?)null);

            // act
            var result = await _sut.GetAsync(id, token);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            await _service.Received(1).GetAsync(id, token);
        }

        [Fact]
        public async Task GetAsync_WhenClientReturnsDto_ReturnsOkWithDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            var dto = _fixture.Create<CategoryDto>();
            _service.GetAsync(id, token).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);

            await _service.Received(1).GetAsync(id, token);
        }

        [Fact]
        public async Task ExistsAsync_Found_ReturnsOk()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            _service.ExistsAsync(id, token).Returns(true);

            // Act
            var result = await _sut.ExistsAsync(id, token);

            // Assert
            var ok = result.Should().BeOfType<OkResult>().Subject;

            await _service.Received(1).ExistsAsync(id, token);
        }

        [Fact]
        public async Task ExistsAsync_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            _service.ExistsAsync(id, token).Returns(false);

            // Act
            var result = await _sut.ExistsAsync(id, token);

            // Assert
            var ok = result.Should().BeOfType<NotFoundResult>().Subject;

            await _service.Received(1).ExistsAsync(id, token);
        }

        [Fact]
        public async Task AddAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<CreateCategoryDto>();

            // Act
            var result = await _sut.AddAsync(request, token);

            // Assert
            result.Result.Should().BeOfType<StatusCodeResult>();
            await _service.Received(1).CreateAsync(request, token);
        }

        [Fact]
        public async Task UpdateAsync_CallsClientAndReturnsNoContent()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<UpdateCategoryDto>();

            // Act
            var result = await _sut.UpdateAsync(id, request, token);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _service.Received(1).UpdateAsync(id, request, token);
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
            await _service.Received(1).DeleteAsync(id, token);
        }
    }
}