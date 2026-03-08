using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Category.Api.Domain.Pagination;
using OrderManagement.Category.Api.Infrastructure.Configuration;
using OrderManagement.Category.Api.Infrastructure.Entities;
using OrderManagement.Category.Api.Infrastructure.Repositories;

namespace OrderManagement.Category.Api.Infrastructure.Tests.Repositories
{
    public sealed class CategoryRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();
        private readonly CategoryRepository _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly CategoryDbContext _dbContext;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CategoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CategoryDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);

            _sut = _fixture.Create<CategoryRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task GetAsync_WhenNotFound_ReturnsNull()
        {
            // Act
            var result = await _sut.GetAsync(id: 999, default);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<Domain.Entities.Category>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenFound_ReturnsMappedCategory()
        {
            // Arrange
            var dbEntity = _fixture.Create<CategoryEntity>();
            _dbContext.Categories.Add(dbEntity);
            await _dbContext.SaveChangesAsync(default);

            var id = dbEntity.Id;

            var mapped = _fixture.Create<Domain.Entities.Category>();
            _mapper.Map<Domain.Entities.Category>(Arg.Any<object>()).Returns(mapped);

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var dbEntity = _fixture.Create<CategoryEntity>();
            _dbContext.Categories.Add(dbEntity);
            await _dbContext.SaveChangesAsync(default);

            var id = dbEntity.Id;

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_AddsMappedEntity_AndPersists()
        {
            // Arrange
            var domain = _fixture.Create<Domain.Entities.Category>();

            // non forzo Id: lo assegna EF
            var dbEntity = _fixture.Create<CategoryEntity>();

            _mapper.Map<CategoryEntity>(domain).Returns(dbEntity);

            // Act
            await _sut.AddAsync(domain, default);

            // Assert
            _mapper.Received(1).Map<CategoryEntity>(domain);
            (await _dbContext.Categories.CountAsync(default)).Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_WhenEntityNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var domain = _fixture.Create<Domain.Entities.Category>();

            // Act
            var act = () => _sut.UpdateAsync(domain, default);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Category with id {domain.Id} not found.");
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_MapsOntoEntity_AndPersists()
        {
            // Arrange
            var existing = _fixture.Create<CategoryEntity>();
            _dbContext.Categories.Add(existing);
            await _dbContext.SaveChangesAsync(default);

            var domain = new Domain.Entities.Category(existing.Id);
            domain.SetName("Updated");

            // Act
            await _sut.UpdateAsync(domain, default);

            // Assert
            _mapper.Received(1).Map(
                Arg.Is<Domain.Entities.Category>(c => c.Id == domain.Id),
                Arg.Is<CategoryEntity>(e => e.Id == domain.Id));

            (await _dbContext.Categories.AnyAsync(x => x.Id == domain.Id, default)).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenEntityNotFound_ThrowsInvalidOperationException()
        {
            // Act
            var act = () => _sut.DeleteAsync(404, default);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Category with id 404 not found.");
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_RemovesEntity_AndPersists()
        {
            var existing = _fixture.Create<CategoryEntity>();
            _dbContext.Categories.Add(existing);
            await _dbContext.SaveChangesAsync(default);

            var id = existing.Id;

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            (await _dbContext.Categories.AnyAsync(x => x.Id == id, default)).Should().BeFalse();
        }
    }
}