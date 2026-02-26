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
    public decimal SubTotal { get; private set; }

    /// <summary>
    /// Order total price.
    /// </summary>
    public decimal Total { get; private set; }

    /// <summary>
    /// Order shipping details.
    /// </summary>
    public ShippingAddress ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// Constructor.
    /// </summary>
    public OrderEntity() { }

    /// <summary>
    /// Constructor overload.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    public OrderEntity(Guid id, int userId, OrderStatus status)
    {
        Id = id;
        UserId = userId;
        Status = status;
    }
}