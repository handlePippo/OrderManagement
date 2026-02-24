using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Factories
{
    public static class OrderItemFactory
    {
        public static IReadOnlyList<OrderItem> Create(int orderId, IReadOnlyList<OrderItemProductInfo> orderItemProductInfo)
        {
            ArgumentNullException.ThrowIfNull(orderItemProductInfo);

            return orderItemProductInfo
                .Select(i => new OrderItem(orderId, i))
                .ToList()
                .AsReadOnly();
        }
    }
}
