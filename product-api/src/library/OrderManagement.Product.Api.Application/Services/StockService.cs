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

        public async Task DecreaseStock(int productId, int quantity, CancellationToken token)
        {
            var product = await _repository.GetAsync(productId, token)
                ?? throw new InvalidOperationException($"Product {productId} not found.");

            if (product.Stock == 0)
            {
                throw new InvalidOperationException($"Product {productId} is already ended.");
            }

            if (product.Stock - quantity <= 0)
            {
                product.ClearStock();
            }
            else
            {
                product.DecreaseStock(quantity);
            }

            await _repository.UpdateAsync(product, token);
        }

        public async Task IncreaseStock(int productId, int quantity, CancellationToken token)
        {
            var product = await _repository.GetAsync(productId, token)
                ?? throw new InvalidOperationException($"Product {productId} not found.");

            product.IncreaseStock(quantity);

            await _repository.UpdateAsync(product, token);
        }
    }
}