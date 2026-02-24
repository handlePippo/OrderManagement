using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;
using System.Collections.Generic;

namespace OrderManagement.Order.Api.Application.Interfaces
{
    public interface IProductCalculationsNormalizer
    {
        (decimal, decimal) NormalizeProductsCalculations(IReadOnlyList<Product> products, IReadOnlyList<OrderItemProductInfo> productsInfo);
        IReadOnlyList<OrderItemProductInfo> NormalizeProductsInfo(IReadOnlyList<Product> products, Dictionary<int, int> itemsToBeAdded);
    }
}