using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Bags
{
    public readonly record struct CompositeBag(IReadOnlyList<OrderItem> OrderItems, Dictionary<int, int> OrderItemsByIdAndQty, IReadOnlyList<int> ProductIds);

}