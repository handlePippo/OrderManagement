using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IOrderNormalizer
    {
        (decimal, decimal) NormalizeOrderItemsCalculations(IReadOnlyList<Product> products, IReadOnlyList<OrderItem> productsInfo);
        IReadOnlyList<OrderItem> NormalizeOrderItems(Domain.Entities.Order order, IReadOnlyList<Product> products, Dictionary<int, int> itemsToBeAdded, bool isUpdate = false);
    }
}