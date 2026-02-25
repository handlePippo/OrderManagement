using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Struct
{
    public readonly record struct CompositeBag(IReadOnlyList<OrderItem> OrderItems, Dictionary<int, int> OrderItemsByIdAndQty, IReadOnlyList<int> ProductIds);
}