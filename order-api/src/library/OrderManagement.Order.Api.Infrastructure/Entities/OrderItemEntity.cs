using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Persistence.Entities;

/// <summary>
/// Order item entity.
/// </summary>
public sealed class OrderItemEntity : EntityBase
{
    /// <summary>
    /// Order id.
    /// </summary>
    public int OrderId { get; private set; }

    /// <summary>
    /// Product details.
    /// </summary>
    public OrderItemProductInfo ProductInfo { get; private set; } = null!;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    private OrderItemEntity() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderId"></param>
    /// <param name="productInfo"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrderItemEntity(int id, int orderId, OrderItemProductInfo productInfo)
        : base(id)
    {
        OrderId = orderId;
        ProductInfo = productInfo ?? throw new ArgumentNullException(nameof(productInfo));
    }
}