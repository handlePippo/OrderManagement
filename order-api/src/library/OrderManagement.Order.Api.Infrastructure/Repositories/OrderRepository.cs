using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Persistence.Configuration;
using OrderManagement.Order.Api.Persistence.Entities;

namespace OrderManagement.Order.Api.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext DbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="mapper"></param>
    /// <param name="currentUserProvider"></param>
    public OrderRepository(OrderDbContext dbContext, IMapper mapper, ICurrentUserProvider currentUserProvider)
    {
        DbContext = dbContext;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<IReadOnlyList<Domain.Entities.Order>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<OrderEntity> entities;
        if (_currentUserProvider.IsAdmin)
        {
            entities = await DbContext
                                      .Orders
                                      .AsNoTracking()
                                      .ToListAsync(cancellationToken);
        }
        else
        {
            entities = await DbContext
                                      .Orders
                                      .Where(o => o.UserId == CurrentUserId)
                                      .AsNoTracking()
                                      .ToListAsync(cancellationToken);
        }

        return _mapper.Map<IReadOnlyList<Domain.Entities.Order>>(entities);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbContext
                .Orders
                .AsNoTracking()
                .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Domain.Entities.Order?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await DbContext
                        .Orders
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return dbEntity is null ? null : _mapper.Map<Domain.Entities.Order>(dbEntity);
    }

    public async Task AddAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<OrderEntity>(entity);

        await DbContext
                .Orders
                .AddAsync(dbEntity, cancellationToken);
    }

    public async Task UpdateAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        var dbEntity = await DbContext
                                .Orders
                                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken)
                                ?? throw new InvalidOperationException($"Order with id {order.Id} not found.");

        order.MarkModified();

        _mapper.Map(order, dbEntity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await DbContext
                                .Orders
                                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
                                ?? throw new InvalidOperationException($"Order with id {id} not found.");

        DbContext.Remove(dbEntity);
    }
}