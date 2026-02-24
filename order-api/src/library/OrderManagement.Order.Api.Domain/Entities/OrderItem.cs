using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Domain.Entities;

/// <summary>
/// Order item.
/// </summary>
public sealed class OrderItem : EntityBase
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
    private OrderItem() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderId"></param>
    /// <param name="productInfo"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrderItem(int id, int orderId, OrderItemProductInfo productInfo)
        : base(id)
    {
        OrderId = orderId;
        ProductInfo = productInfo ?? throw new ArgumentNullException(nameof(productInfo));
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="productInfo"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrderItem(int orderId, OrderItemProductInfo productInfo)
    {
        OrderId = orderId;
        ProductInfo = productInfo ?? throw new ArgumentNullException(nameof(productInfo));
    }
}