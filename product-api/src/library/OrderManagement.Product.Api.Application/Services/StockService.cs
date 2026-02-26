using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Application.Interfaces;
using OrderManagement.Product.Api.Application.Repositories;

namespace OrderManagement.Product.Api.Application.Services
{
    public sealed class StockService : IStockService
    {
        private readonly IProductRepository _repository;
        public StockService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task DecreaseStock(UpdateStockDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var (products, productsByIdAndQty) = await GetProductsAsync(dto, token);

            foreach (var product in products)
            {
                if (product.Stock == 0)
                {
                    throw new InvalidOperationException($"Product {product.Id} is not avalaible.");
                }

                var qty = productsByIdAndQty[product.Id];
                if (product.Stock - qty <= 0)
                {
                    product.ClearStock();
                }
                else
                {
                    product.DecreaseStock(qty);
                }
            }

            var productsById = products.ToDictionary(g => g.Id);

            await _repository.UpdateRangeAsync(productsById, token);
        }

        public async Task IncreaseStock(UpdateStockDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var (products, productsByIdAndQty) = await GetProductsAsync(dto, token);

            foreach (var product in products)
            {
                product.IncreaseStock(productsByIdAndQty[product.Id]);
            }

            var productsById = products.ToDictionary(g => g.Id);

            await _repository.UpdateRangeAsync(productsById, token);
        }

        private async Task<(IReadOnlyList<Domain.Entities.Product>, Dictionary<int, int>)> GetProductsAsync(UpdateStockDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var duplicatedIds = dto.Lines
                                        .GroupBy(x => x.ProductId)
                                        .Where(g => g.Count() > 1)
                                        .Select(g => g.Key)
                                        .ToArray();

            if (duplicatedIds.Length > 0)
            {
                throw new InvalidOperationException($"Duplicated product ids: {string.Join(", ", duplicatedIds)}");
            }

            var productsByIdAndQty = dto.Lines.ToDictionary(x => x.ProductId, q => q.Quantity);

            var productIds = new Domain.Entities.ProductRange(productsByIdAndQty.Keys.ToList());

            var products = await _repository.GetRangeAsync(productIds, token);

            if (products is null || products.Count == 0 || products.Count != productsByIdAndQty.Count)
            {
                throw new InvalidOperationException($"One or more product not found.");
            }

            return (products, productsByIdAndQty);
        }
    }
}