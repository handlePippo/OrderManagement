using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Product.Api.Application.Repositories;
using OrderManagement.Product.Api.Domain.Entities;
using OrderManagement.Product.Api.Infrastructure.Configuration;
using OrderManagement.Product.Api.Infrastructure.Entities;

namespace OrderManagement.Product.Api.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ProductDbContext DbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="mapper"></param>
    public ProductRepository(ProductDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Domain.Entities.Product>> ListAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await DbContext
                                .Products
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<Domain.Entities.Product>>(dbEntities);
    }

    public async Task<Domain.Entities.Product?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Products
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity is null ? null : _mapper.Map<Domain.Entities.Product>(entity);
    }

    public async Task<IReadOnlyList<Domain.Entities.Product>> GetRangeAsync(ProductRange range, CancellationToken cancellationToken = default)
    {
        var entities = await DbContext
                            .Products
                            .AsNoTracking()
                            .Where(e => range.Ids.Contains(e.Id))
                            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<Domain.Entities.Product>>(entities);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return DbContext
                .Products
                .AsNoTracking()
                .AnyAsync(e => e.Id == id, ct);
    }

    public async Task AddAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<ProductEntity>(product);

        await DbContext
                .Products
                .AddAsync(entity, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Products
                            .FirstOrDefaultAsync(x => x.Id == product.Id, cancellationToken)
                            ?? throw new InvalidOperationException($"Product with id {product.Id} not found.");

        _mapper.Map(product, entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateRangeAsync(Dictionary<int, Domain.Entities.Product> productsById, CancellationToken cancellationToken = default)
    {
        var entities = await DbContext
                            .Products
                            .Where(x => productsById.Keys.Contains(x.Id))
                            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            if (!productsById.TryGetValue(entity.Id, out var product))
            {
                throw new InvalidOperationException($"One or more product not found.");
            }

            _mapper.Map(product, entity);
        }

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Products
                            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                            ?? throw new InvalidOperationException($"Product with id {id} not found.");

        DbContext.Remove(entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}