using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Persistence.Entities;

/// <summary>
/// Order entity.
/// </summary>
public sealed class OrderEntity : EntityBase
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
    /// Constructor for EF / Automapper.
    /// </summary>
    private OrderEntity() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="shippingAddress"></param>
    /// <param name="items"></param>
    /// <exception cref="ArgumentException"></exception>
    public OrderEntity(int id, int userId, OrderStatus status, decimal subtotal, decimal total, ShippingAddress shippingAddress)
        : base(id)
    {
        UserId = userId;
        Status = status;
        Subtotal = subtotal;
        Total = total;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
    }
}