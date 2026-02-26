namespace OrderManagement.Order.Api.Infrastructure.Entities;

/// <summary>
/// Order item entity.
/// </summary>
public sealed class OrderItemEntity : EntityBase
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; private set; }

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

    /// <summary>
    /// Constructor.
    /// </summary>
    public OrderItemEntity() { }

    /// <summary>
    /// Constructor overload.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderId"></param>
    /// <param name="productId"></param>
    public OrderItemEntity(int id, Guid orderId, int productId)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
    }
}