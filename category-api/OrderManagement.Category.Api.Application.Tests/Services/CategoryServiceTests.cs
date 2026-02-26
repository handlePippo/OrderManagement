using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Category.Api.Application.DTOs;
using OrderManagement.Category.Api.Application.Repositories;
using OrderManagement.Category.Api.Application.Services;

namespace OrderManagement.Category.Api.Application.Tests.Services
{
    public sealed class CategoryServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly CategoryService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICategoryRepository _repository = Substitute.For<ICategoryRepository>();

        public CategoryServiceTests()
        {
            _fixture.Inject(_mapper);
            _fixture.Inject(_repository);

            _sut = _fixture.Create<CategoryService>();
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            _repository.ListAsync(default).Returns((IReadOnlyList<Domain.Entities.Category>?)null!);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<IReadOnlyList<CategoryDto>>(default!);
        }

        [Fact]
        public async Task ListAsync_WhenRepositoryReturnsCategories_MapsAndReturnsDtos()
        {
            // Arrange
            var categories = _fixture.CreateMany<Domain.Entities.Category>(3).ToList().AsReadOnly();
            var dtos = _fixture.CreateMany<CategoryDto>(3).ToList().AsReadOnly();

            _repository.ListAsync(default).Returns(categories);
            _mapper.Map<IReadOnlyList<CategoryDto>>(categories).Returns(dtos);

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().BeSameAs(dtos);
            await _repository.Received(1).ListAsync(default);
            _mapper.Received(1).Map<IReadOnlyList<CategoryDto>>(categories);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _repository.GetAsync(id, default).Returns((Domain.Entities.Category?)null);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<CategoryDto>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenRepositoryReturnsCategory_MapsAndReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();

            var category = _fixture.Create<Domain.Entities.Category>();
            var dto = _fixture.Create<CategoryDto>();

            _repository.GetAsync(id, default).Returns(category);
            _mapper.Map<CategoryDto>(category).Returns(dto);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(dto);
            await _repository.Received(1).GetAsync(id, default);
            _mapper.Received(1).Map<CategoryDto>(category);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var act = () => _sut.CreateAsync(null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoToEntity_AndAddsToRepository()
        {
            // Arrange
            var dto = _fixture.Create<CreateCategoryDto>();
            var entity = _fixture.Create<Domain.Entities.Category>();

            _mapper.Map<Domain.Entities.Category>(dto).Returns(entity);

            // Act
            await _sut.CreateAsync(dto, default);

            // Assert
            _mapper.Received(1).Map<Domain.Entities.Category>(dto);
            await _repository.Received(1).AddAsync(entity, default);
        }

        [Fact]
        public async Task DeleteAsync_DelegatesToRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            await _repository.Received(1).DeleteAsync(id, default);
        }

        [Fact]
        public async Task ExistsAsync_DelegatesToRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _repository.ExistsAsync(id, default).Returns(true);

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
            await _repository.Received(1).ExistsAsync(id, default);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            var act = () => _sut.UpdateAsync(id, null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact]
        public async Task UpdateAsync_CreatesCategoryWithId_AndUpdatesRepository()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateCategoryDto>();

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            await _repository.Received(1).UpdateAsync(Arg.Is<Domain.Entities.Category>(c => c.Id == id), default);
        }
    }
}