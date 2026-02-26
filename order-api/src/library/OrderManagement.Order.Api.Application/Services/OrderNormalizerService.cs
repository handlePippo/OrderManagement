using OrderManagement.Order.Api.Application.Factories;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.Services
{
    /// <summary>
    /// Product normalizer.
    /// </summary>
    public sealed class OrderNormalizerService : IOrderNormalizerService
    {
        public IReadOnlyList<OrderItem> NormalizeOrderItems(Domain.Entities.Order order, IReadOnlyList<Product> products, Dictionary<int, int> productsToBeAdded, bool isUpdate = false)
        {
            ArgumentNullException.ThrowIfNull(products);
            ArgumentNullException.ThrowIfNull(productsToBeAdded);

            var result = new List<OrderItem>();
            foreach (var product in products)
            {
                if (!productsToBeAdded.TryGetValue(product.Id, out var qty))
                {
                    throw new ValidationException($"Product {product.Id} not found.");
                }

                var orderItem = OrderItemFactory.Create(order.Id, product.Id, product.Name, product.Price, qty);

                if (isUpdate)
                {
                    orderItem.MarkModified();
                }

                result.Add(orderItem);
            }

            return result;
        }

        public (decimal, decimal, ProductStock) NormalizeOrderItemsCalculations(IReadOnlyList<Product> products, IReadOnlyList<OrderItem> linesToBeAdded)
        {
            ArgumentNullException.ThrowIfNull(products);
            ArgumentNullException.ThrowIfNull(linesToBeAdded);

            var productsByIdAndPrice = products.ToDictionary(q => q.Id, q => q.Price);

            var productStock = new ProductStock();
            decimal subTotal = 0;
            foreach (var line in linesToBeAdded)
            {
                if (!productsByIdAndPrice.TryGetValue(line.ProductId, out var price))
                {
                    throw new ValidationException($"Product {line.ProductId} not found.");
                }

                productStock.Lines.Add(new ProductStockLine
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                });

                subTotal += line.Quantity * price;
            }

            var total = subTotal;
            return new(subTotal, total, productStock);
        }
    }
}