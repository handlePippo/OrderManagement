using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OrderManagement.Product.Api.Domain.Entities;
using OrderManagement.Product.Api.Infrastructure.Configuration;
using OrderManagement.Product.Api.Infrastructure.Entities;
using OrderManagement.Product.Api.Infrastructure.Repositories;

namespace OrderManagement.Product.Api.Infrastructure.Tests.Repositories
{
    public sealed class ProductRepositoryTests : IDisposable
    {
        private readonly Fixture _fixture = new();

        private ProductRepository _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ProductDbContext _dbContext;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ProductDbContext(options);

            _fixture.Inject(_dbContext);
            _fixture.Inject(_mapper);

            _sut = _fixture.Create<ProductRepository>();
        }

        public void Dispose() => _dbContext.Dispose();

        [Fact]
        public async Task ListAsync_ReturnsMappedProducts()
        {
            // Arrange
            var ct = CancellationToken.None;

            var dbEntities = _fixture.CreateMany<ProductEntity>().ToList();
            _dbContext.Products.AddRange(dbEntities);
            await _dbContext.SaveChangesAsync(ct);

            var mapped = _fixture.CreateMany<Domain.Entities.Product>(2).ToList().AsReadOnly();

            _mapper.Map<IReadOnlyList<Domain.Entities.Product>>(Arg.Any<object>())
                  .Returns(mapped);

            // Act
            var result = await _sut.ListAsync(ct);

            // Assert
            result.Should().BeSameAs(mapped);
            _mapper.Received(1)
                .Map<IReadOnlyList<Domain.Entities.Product>>(
                    Arg.Is<object>(o => o is List<ProductEntity>));
        }

        [Fact]
        public async Task GetAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _sut.GetAsync(id: 999, ct);

            // Assert
            result.Should().BeNull();
            _mapper.DidNotReceiveWithAnyArgs().Map<Domain.Entities.Product>(default!);
        }

        [Fact]
        public async Task GetAsync_WhenFound_ReturnsMappedProduct()
        {
            // Arrange
            var ct = CancellationToken.None;

            var entity = _fixture.Build<ProductEntity>().Create();

            _dbContext.Products.Add(entity);
            await _dbContext.SaveChangesAsync(ct);

            var mapped = _fixture.Create<Domain.Entities.Product>();

            _mapper.Map<Domain.Entities.Product>(Arg.Any<object>())
                  .Returns(mapped);

            // Act
            var result = await _sut.GetAsync(entity.Id, ct);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task GetRangeAsync_FiltersByIds_AndReturnsMappedList()
        {
            // Arrange
            var ct = CancellationToken.None;

            // seed controllato: serve avere per forza Id 1 e 3
            var e1 = _fixture.Build<ProductEntity>().Create();
            var e2 = _fixture.Build<ProductEntity>().Create();
            var e3 = _fixture.Build<ProductEntity>().Create();

            _dbContext.Products.AddRange(e1, e2, e3);
            await _dbContext.SaveChangesAsync(ct);

            var range = new GetProductRange(new[] { 1, 3 });

            var mapped = _fixture.CreateMany<Domain.Entities.Product>(2).ToList().AsReadOnly();

            _mapper.Map<IReadOnlyList<Domain.Entities.Product>>(Arg.Any<object>())
                  .Returns(mapped);

            // Act
            var result = await _sut.GetRangeAsync(range, ct);

            // Assert
            result.Should().BeSameAs(mapped);
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var ct = CancellationToken.None;

            var entity = _fixture.Create<ProductEntity>();

            _dbContext.Products.Add(entity);
            await _dbContext.SaveChangesAsync(ct);

            // Act
            var result = await _sut.ExistsAsync(entity.Id, ct);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_AddsMappedEntity_AndPersists()
        {
            // Arrange
            var ct = CancellationToken.None;

            var domain = _fixture.Create<Domain.Entities.Product>();
            var entity = _fixture.Create<ProductEntity>();

            _mapper.Map<ProductEntity>(domain).Returns(entity);

            // Act
            await _sut.AddAsync(domain, ct);

            // Assert
            _mapper.Received(1).Map<ProductEntity>(domain);
            (await _dbContext.Products.CountAsync(ct)).Should().Be(1);
            (await _dbContext.Products.AnyAsync(x => x.Id == entity.Id, ct)).Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_WhenEntityNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var ct = CancellationToken.None;

            var domain = _fixture.Build<Domain.Entities.Product>().Create();

            // Act
            var act = () => _sut.UpdateAsync(domain, ct);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Product with id {domain.Id} not found.");
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_MapsOntoEntity_AndPersists()
        {
            // Arrange
            var ct = CancellationToken.None;

            var existing = _fixture.Create<ProductEntity>();

            _dbContext.Products.Add(existing);
            await _dbContext.SaveChangesAsync(ct);

            var domain = new Domain.Entities.Product(existing.Id);
            domain.SetCategoryId(existing.CategoryId);
            domain.SetDescription(existing.Description);
            domain.SetName("Updated");
            domain.SetSku(existing.Sku);
            domain.SetPrice(existing.Price);

            // Act
            await _sut.UpdateAsync(domain, ct);

            // Assert
            _mapper.Received(1).Map(
                Arg.Is<Domain.Entities.Product>(p => p.Id == domain.Id),
                Arg.Is<ProductEntity>(e => e.Id == domain.Id));

            (await _dbContext.Products.AnyAsync(x => x.Id == domain.Id, ct)).Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenEntityNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var act = () => _sut.DeleteAsync(404, ct);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Product with id 404 not found.");
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_RemovesEntity_AndPersists()
        {
            // Arrange
            var ct = CancellationToken.None;

            var entity = _fixture.Build<ProductEntity>().Create();

            _dbContext.Products.Add(entity);
            await _dbContext.SaveChangesAsync(ct);

            // Act
            await _sut.DeleteAsync(entity.Id, ct);

            // Assert
            (await _dbContext.Products.AnyAsync(x => x.Id == entity.Id, ct)).Should().BeFalse();
        }
    }
}