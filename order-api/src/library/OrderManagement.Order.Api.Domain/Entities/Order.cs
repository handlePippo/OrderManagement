using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Domain.Entities;

/// <summary>
/// Order.
/// </summary>
public sealed class Order : EntityBase
{
    /// <summary>
    /// Id of the user that requests the order.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Status of the order.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Order subtotal.
    /// </summary>
    public decimal Subtotal { get; private set; }

    /// <summary>
    /// Order total price.
    /// </summary>
    public decimal Total { get; private set; }

    /// <summary>
    /// Order shipping details.
    /// </summary>
    public ShippingAddress ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// Items in the order.
    /// </summary>
    public IReadOnlyList<OrderItem> Items { get; private set; } = Array.Empty<OrderItem>()!;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    public Order() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    /// <exception cref="ArgumentException"></exception>
    public Order(int id, int userId, OrderStatus status)
        : base(id)
    {
        UserId = userId;
        Status = status;
    }

    /// <summary>
    /// Sets user id.
    /// </summary>
    /// <param name="userId"></param>
    public void SetUserId(int userId) => UserId = userId;

    /// <summary>
    /// Sets order status.
    /// </summary>
    /// <param name="status"></param>
    public void SetStatus(OrderStatus status) => Status = status;

    /// <summary>
    /// Sets shipping address.
    /// </summary>
    /// <param name="shippingAddress"></param>
    public void SetShippingAddress(ShippingAddress shippingAddress)
    {
        ArgumentNullException.ThrowIfNull(shippingAddress);

        ShippingAddress = shippingAddress;
    }

    /// <summary>
    /// Sets items.
    /// </summary>
    /// <param name="items"></param>
    public void SetItems(IReadOnlyList<OrderItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
    }

    /// <summary>
    /// Sets totals.
    /// </summary>
    /// <param name="subtotal"></param>
    /// <param name="total"></param>
    public void SetTotals(decimal subtotal, decimal total)
    {
        Subtotal = subtotal;
        Total = total;
    }

    /// <summary>
    /// Marks the order modified.
    /// </summary>
    public void MarkModified() => ModifiedAt = DateTime.UtcNow;
}