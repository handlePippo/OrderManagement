namespace OrderManagement.Order.Api.Domain.Entities;

/// <summary>
/// Order item.
/// </summary>
public sealed class OrderItem : EntityBase
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
    public int ProductId { get; init; }

    /// <summary>
    /// Qty.
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
    /// Line total.
    /// </summary>
    public decimal LineTotal => UnitPrice * Quantity;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    public OrderItem() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="orderId"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrderItem(int productId, Guid orderId)
    {
        ProductId = productId;
        OrderId = orderId;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrderItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    /// <summary>
    /// Marks the order modified.
    /// </summary>
    public void MarkModified() => ModifiedAt = DateTime.UtcNow;
}