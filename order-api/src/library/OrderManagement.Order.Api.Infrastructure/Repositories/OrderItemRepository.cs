using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Infrastructure.Configuration;
using OrderManagement.Order.Api.Infrastructure.Entities;

namespace OrderManagement.Order.Api.Infrastructure.Repositories;

public sealed class OrderItemRepository : IOrderItemRepository
{
    private readonly OrderDbContext DbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="mapper"></param>
    public OrderItemRepository(OrderDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddRangeAsync(IReadOnlyList<OrderItem> entities, CancellationToken cancellationToken = default)
    {
        var dbEntities = _mapper.Map<IReadOnlyList<OrderItemEntity>>(entities);

        await DbContext
                       .OrderItems
                       .AddRangeAsync(dbEntities, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetRangeByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var entities = await DbContext
                                      .OrderItems
                                      .AsNoTracking()
                                      .Where(o => o.OrderId == orderId)
                                      .ToListAsync(cancellationToken)
                                      ?? throw new InvalidOperationException($"Order with id {orderId} not found.");

        return _mapper.Map<IReadOnlyList<OrderItem>>(entities);
    }

    public async Task<IReadOnlyList<OrderItem>> GetRangeByOrderIdAsync(IReadOnlyList<Guid> orderIds, CancellationToken cancellationToken = default)
    {
        var entities = await DbContext
                              .OrderItems
                              .AsNoTracking()
                              .Where(o => orderIds.Contains(o.OrderId))
                              .ToListAsync(cancellationToken)
                              ?? throw new InvalidOperationException($"One or more orderId does not belong to any order.");

        return _mapper.Map<IReadOnlyList<OrderItem>>(entities);
    }

    public async Task DeleteRangeByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var entities = await DbContext
                                      .OrderItems
                                      .Where(o => o.OrderId == orderId)
                                      .ToListAsync(cancellationToken)
                                      ?? throw new InvalidOperationException($"Order with id {orderId} not found.");

        DbContext.RemoveRange(entities);
    }
}
