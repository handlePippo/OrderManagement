using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Bags
{
    public readonly record struct NormalizedOrderBag(decimal SubTotal, decimal Total, IReadOnlyList<Product> Products, Dictionary<int, int> OrderItemsToBeProcessed, ProductStock ProductStock);
}