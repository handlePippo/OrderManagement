using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Factories
{
    public static class OrderFactory
    {
        public static Domain.Entities.Order Create(int userId, ShippingAddress shippingAddress, decimal subTotal, decimal total)
        {
            ArgumentNullException.ThrowIfNull(shippingAddress);

            var order = new Domain.Entities.Order();
            order.SetId(Guid.NewGuid());
            order.SetUserId(userId);
            order.SetStatus(OrderStatus.Pending);
            order.SetShippingAddress(shippingAddress);
            order.SetTotals(subTotal, total);
            return order;
        }
    }
}