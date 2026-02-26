using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Application.Repositories;
using OrderManagement.Product.Api.Application.Services;
using OrderManagement.Product.Api.Domain.Entities;

namespace OrderManagement.Product.Api.Application.Tests.Services
{
    public sealed class ProductServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly ProductService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IProductRepository _repository = Substitute.For<IProductRepository>();

        public ProductServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_repository);

            _sut = _fixture.Create<ProductService>();
        }

        [Fact]
        public async Task ExistsAsync_DelegatesToRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var token = CancellationToken.None;

            _repository.ExistsAsync(id, token).Returns(true);

            // Act
            var result = await _sut.ExistsAsync(id, token);

            // Assert
            result.Should().BeTrue();
            await _repository.Received(1).ExistsAsync(id, token);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var token = CancellationToken.None;

            _repository.ListAsync(token).Returns((IReadOnlyList<Domain.Entities.Product>?)null!);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<IReadOnlyList<ProductDto>>(default!);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsProducts_MapsAndReturnsDtos()
        {
            // Arrange
            var token = CancellationToken.None;

            var products = _fixture.CreateMany<Domain.Entities.Product>(3).ToList().AsReadOnly();
            var dtos = _fixture.CreateMany<ProductDto>(3).ToList().AsReadOnly();

            _repository.ListAsync(token).Returns(products);
            _mapper.Map<IReadOnlyList<ProductDto>>(products).Returns(dtos);

            // Act
            var result = await _sut.ListAsync(token);

            // Assert
            result.Should().BeSameAs(dtos);
            await _repository.Received(1).ListAsync(token);
            _mapper.Received(1).Map<IReadOnlyList<ProductDto>>(products);
        }

        [Fact]
        public async Task GetRangeAsync_MapsRangeDto_CallsRepository_AndMapsResult()
        {
            // Arrange
            var token = CancellationToken.None;

            var dto = _fixture.Create<GetProductRangeDto>();
            var range = _fixture.Create<ProductRange>();

            var products = _fixture.CreateMany<Domain.Entities.Product>(2).ToList().AsReadOnly();
            var dtos = _fixture.CreateMany<ProductDto>(2).ToList().AsReadOnly();

            _mapper.Map<ProductRange>(dto).Returns(range);
            _repository.GetRangeAsync(range, token).Returns(products);
            _mapper.Map<IReadOnlyList<ProductDto>>(products).Returns(dtos);

            // Act
            var result = await _sut.GetRangeAsync(dto, token);

            // Assert
            result.Should().BeSameAs(dtos);

            _mapper.Received(1).Map<ProductRange>(dto);
            await _repository.Received(1).GetRangeAsync(range, token);
            _mapper.Received(1).Map<IReadOnlyList<ProductDto>>(products);
        }

        [Fact]
        public async Task GetRangeAsync_WhenRepositoryReturnsNull_ReturnsEmptyArray_AndDoesNotMapProducts()
        {
            // Arrange
            var token = CancellationToken.None;

            var dto = _fixture.Create<GetProductRangeDto>();
            var range = _fixture.Create<ProductRange>();

            _mapper.Map<ProductRange>(dto).Returns(range);
            _repository.GetRangeAsync(range, token).Returns((IReadOnlyList<Domain.Entities.Product>?)null!);

            // Act
            var result = await _sut.GetRangeAsync(dto, token);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mapper.Received(1).Map<ProductRange>(dto);
            await _repository.Received(1).GetRangeAsync(range, token);

            _mapper.DidNotReceiveWithAnyArgs().Map<IReadOnlyList<ProductDto>>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();

            _repository.GetAsync(id, token).Returns((Domain.Entities.Product?)null);

            // Act
            var result = await _sut.GetAsync(id, token);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<ProductDto>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsProduct_MapsAndReturnsDto()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();

            var product = _fixture.Create<Domain.Entities.Product>();
            var dto = _fixture.Create<ProductDto>();

            _repository.GetAsync(id, token).Returns(product);
            _mapper.Map<ProductDto>(product).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, token);

            // Assert
            result.Should().BeSameAs(dto);
            await _repository.Received(1).GetAsync(id, token);
            _mapper.Received(1).Map<ProductDto>(product);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var token = CancellationToken.None;

            // Act
            var act = () => _sut.CreateAsync(null!, token);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoToEntity_AndAddsToRepository()
        {
            // Arrange
            var token = CancellationToken.None;

            var dto = _fixture.Create<CreateProductDto>();
            var entity = _fixture.Create<Domain.Entities.Product>();

            _mapper.Map<Domain.Entities.Product>(dto).Returns(entity);

            // Act
            await _sut.CreateAsync(dto, token);

            // Assert
            _mapper.Received(1).Map<Domain.Entities.Product>(dto);
            await _repository.Received(1).AddAsync(entity, token);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();

            // Act
            var act = () => _sut.UpdateAsync(id, null!, token);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateProductDto>();

            _repository.GetAsync(id, token).Returns((Domain.Entities.Product?)null);

            // Act
            var act = () => _sut.UpdateAsync(id, dto, token);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Product {id} not found.");

            await _repository.Received(1).GetAsync(id, token);
            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductExists_AppliesPatch_AndUpdatesRepository()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateProductDto>();

            var product = _fixture.Create<Domain.Entities.Product>();

            _repository.GetAsync(id, token).Returns(product);

            // Act
            await _sut.UpdateAsync(id, dto, token);

            // Assert
            await _repository.Received(1).GetAsync(id, token);
            await _repository.Received(1).UpdateAsync(product, token);
        }

        [Fact]
        public async Task DeleteAsync_DelegatesToRepository()
        {
            // Arrange
            var token = CancellationToken.None;
            var id = _fixture.Create<int>();

            // Act
            await _sut.DeleteAsync(id, token);

            // Assert
            await _repository.Received(1).DeleteAsync(id, token);
        }
    }
}