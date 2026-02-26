using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IOrderNormalizerService
    {
        (decimal, decimal, ProductStock) NormalizeOrderItemsCalculations(IReadOnlyList<Product> products, IReadOnlyList<OrderItem> productsInfo);
        IReadOnlyList<OrderItem> NormalizeOrderItems(Domain.Entities.Order order, IReadOnlyList<Product> products, Dictionary<int, int> itemsToBeAdded, bool isUpdate = false);
    }
}