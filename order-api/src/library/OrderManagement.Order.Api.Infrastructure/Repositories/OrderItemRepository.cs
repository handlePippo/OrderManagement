using AutoMapper;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Persistence.Configuration;
using OrderManagement.Order.Api.Persistence.Entities;

namespace OrderManagement.Order.Api.Persistence.Repositories;

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

    public async Task AddRangeAsync(IReadOnlyList<Domain.Entities.OrderItem> entities, CancellationToken cancellationToken = default)
    {
        var dbEntities = _mapper.Map<IReadOnlyList<OrderItemEntity>>(entities);

        await DbContext
            .OrderItems
            .AddRangeAsync(dbEntities, cancellationToken);
    }
}
