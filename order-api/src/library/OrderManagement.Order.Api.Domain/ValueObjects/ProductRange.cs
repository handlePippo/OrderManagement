namespace OrderManagement.Order.Api.Domain.ValueObjects;

public record ProductRange
{
    public IReadOnlyList<int> OrderIds { get; set; }

    public ProductRange(IReadOnlyList<int> orderIds)
    {
        OrderIds = orderIds ?? throw new ArgumentNullException(nameof(orderIds));
    }
}