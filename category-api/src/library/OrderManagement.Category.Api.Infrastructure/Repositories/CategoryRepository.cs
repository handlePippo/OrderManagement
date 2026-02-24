using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Category.Api.Application.Repositories;
using OrderManagement.Category.Api.Infrastructure.Configuration;
using OrderManagement.Category.Api.Infrastructure.Entities;

namespace OrderManagement.Category.Api.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly CategoryDbContext DbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="mapper"></param>
    public CategoryRepository(CategoryDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Domain.Entities.Category>> ListAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await DbContext
                                .Categories
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<Domain.Entities.Category>>(dbEntities);
    }

    public async Task<Domain.Entities.Category?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await DbContext
                                .Categories
                                .AsNoTracking()
                                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return dbEntity is null ? null : _mapper.Map<Domain.Entities.Category>(dbEntity);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return DbContext
                .Categories
                .AsNoTracking()
                .AnyAsync(e => e.Id == id, ct);
    }

    public async Task AddAsync(Domain.Entities.Category entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<CategoryEntity>(entity);

        await DbContext
                .Categories
                .AddAsync(dbEntity, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Domain.Entities.Category category, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Categories
                            .FirstOrDefaultAsync(x => x.Id == category.Id, cancellationToken)
                            ?? throw new InvalidOperationException($"Category with id {category.Id} not found.");

        _mapper.Map(category, entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Categories
                            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                            ?? throw new InvalidOperationException($"Category with id {id} not found.");

        DbContext.Remove(entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}