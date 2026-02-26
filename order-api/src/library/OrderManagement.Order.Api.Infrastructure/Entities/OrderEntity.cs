using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Infrastructure.Entities;

/// <summary>
/// Order entity.
/// </summary>
public sealed class OrderEntity : EntityBase
{
    /// <summary>
    /// Entity id.
    /// </summary>
    public Guid Id { get; private set; }

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
}