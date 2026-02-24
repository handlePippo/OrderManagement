using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Domain.Entities;

namespace OrderManagement.Order.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this Domain.Entities.Order order, decimal subTotal, decimal total, ShippingAddress shippingAddress)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(shippingAddress);

            order.SetShippingAddress(shippingAddress);
            order.SetTotals(subTotal, total);
            order.MarkModified();
        }
    }
}