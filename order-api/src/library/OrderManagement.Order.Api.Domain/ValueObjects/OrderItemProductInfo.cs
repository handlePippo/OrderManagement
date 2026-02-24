namespace OrderManagement.Order.Api.Domain.ValueObjects;

public sealed class OrderItemProductInfo
{
    public int ProductId { get; private init; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    private OrderItemProductInfo() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="productId"></param>
    public OrderItemProductInfo(int productId)
    {
        ProductId = productId;
    }
}