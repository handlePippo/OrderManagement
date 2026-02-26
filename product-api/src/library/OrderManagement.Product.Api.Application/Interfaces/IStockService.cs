namespace OrderManagement.Product.Api.Application.Services
{
    public interface IStockService
    {
        Task IncreaseStock(int productId, int quantity, CancellationToken token);
        Task DecreaseStock(int productId, int quantity, CancellationToken token);
    }
}