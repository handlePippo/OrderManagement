using OrderManagement.Order.Api.Application.DTOs.Orders.Update;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Extensions
{
    public static class UpdateExtensions
    {
        public static void ApplyPatchFrom(this Domain.Entities.Order order, UpdateOrderDto dto, ShippingAddress shippingAddress)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(shippingAddress);

            if (dto.ShippingAddress is null)
            {
                return;
            }

            var updateCounter = 0;
            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress.ShipAddress))
            {
                shippingAddress.SetShipAddress(dto.ShippingAddress.ShipAddress);
                updateCounter++;
            }

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress.ShipCity))
            {
                shippingAddress.SetShipCity(dto.ShippingAddress.ShipCity);
                updateCounter++;
            }

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress.ShipPostalCode))
            {
                shippingAddress.SetShipPostalCode(dto.ShippingAddress.ShipPostalCode);
                updateCounter++;
            }

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress.ShipCountryCode))
            {
                shippingAddress.SetShipCountryCode(dto.ShippingAddress.ShipCountryCode);
                updateCounter++;
            }

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress.ShipPhoneNumber))
            {
                shippingAddress.SetShipPhoneNumber(dto.ShippingAddress.ShipPhoneNumber);
                updateCounter++;
            }

            if (updateCounter > 0)
            {
                order.SetShippingAddress(shippingAddress);
                order.MarkModified();
            }
        }
    }
}