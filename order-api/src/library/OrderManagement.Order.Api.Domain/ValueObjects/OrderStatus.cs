namespace OrderManagement.Order.Api.Domain.ValueObjects;

/// <summary>
/// Order status.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Pendingo order (requested).
    /// </summary>
    Pending,
    /// <summary>
    /// Completed order (payed).
    /// </summary>
    Completed,
    /// <summary>
    /// Deleted order.
    /// </summary>
    Deleted
}