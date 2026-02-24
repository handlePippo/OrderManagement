namespace OrderManagement.Order.Api.Persistence.Entities;

/// <summary>
/// Order item entity.
/// </summary>
public sealed class OrderItemEntity : EntityBase
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order id.
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Product id.
    /// </summary>
    public int ProductId { get; private init; }

    /// <summary>
    /// Quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Product name.
    /// </summary>
    public string ProductName { get; set; } = null!;

    /// <summary>
    /// Unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }
}