using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Bags
{
    public readonly record struct UpdateBag(IReadOnlyList<OrderItem> OrderItemsToBeUpdated, ProductStock OldStockToUpdate, ProductStock NewStockToUpdate, bool ExecuteUpdate);

}