using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.Services
{
    public sealed class ProductCalculationsNormalizer : IProductCalculationsNormalizer
    {
        public (decimal, decimal) NormalizeProductsCalculations(IReadOnlyList<Product> products, IReadOnlyList<OrderItemProductInfo> productsToBeAdded)
        {
            ArgumentNullException.ThrowIfNull(products);
            ArgumentNullException.ThrowIfNull(productsToBeAdded);

            var productsByIdAndPrice = products.ToDictionary(q => q.Id, q => q.Price);

            decimal subtotal = 0;
            foreach (var item in productsToBeAdded)
            {
                if (!productsByIdAndPrice.TryGetValue(item.ProductId, out var price))
                {
                    throw new ValidationException($"Product {item.ProductId} not found.");
                }

                subtotal += item.Quantity * price;
            }

            var total = subtotal;
            return new(subtotal, total);
        }

        public IReadOnlyList<OrderItemProductInfo> NormalizeProductsInfo(IReadOnlyList<Product> products, Dictionary<int, int> productsToBeAdded)
        {
            ArgumentNullException.ThrowIfNull(products);
            ArgumentNullException.ThrowIfNull(productsToBeAdded);

            var result = new List<OrderItemProductInfo>();
            foreach (var product in products)
            {
                if (!productsToBeAdded.TryGetValue(product.Id, out var qty))
                {
                    throw new ValidationException($"Product {product.Id} not found.");
                }

                result.Add(new OrderItemProductInfo(product.Id)
                {
                    ProductName = product.Name,
                    Quantity = qty,
                    UnitPrice = product.Price
                });
            }

            return result;
        }
    }
}