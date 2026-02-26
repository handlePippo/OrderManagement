using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Factories
{
    public static class OrderItemFactory
    {
        public static OrderItem Create(Guid orderId, int productId, string name, decimal price, int qty)
        {
            ArgumentNullException.ThrowIfNull(orderId);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (productId <= 0)
            {
                throw new ArgumentException("Invalid product id", nameof(productId));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Invalid price", nameof(price));
            }

            if (qty <= 0)
            {
                throw new ArgumentException("Invalid quantity", nameof(qty));
            }

            return new OrderItem(productId, orderId)
            {
                ProductName = name,
                Quantity = qty,
                UnitPrice = price
            };
        }
    }
}