using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this Domain.Entities.Order order, decimal subTotal, decimal total)
        {
            ArgumentNullException.ThrowIfNull(order);

            order.SetTotals(subTotal, total);
            order.MarkModified();
        }

        public static void ApplyPatchFrom(this Domain.Entities.Order order, ShippingAddress shippingAddress)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(shippingAddress);

            order.SetShippingAddress(shippingAddress);
            order.MarkModified();
        }
    }
}