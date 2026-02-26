using OrderManagement.Product.Api.Application.DTOs;

namespace OrderManagement.Product.Api.Application.Interfaces
{
    public interface IStockService
    {
        Task IncreaseStock(UpdateStockDto dto, CancellationToken token);
        Task DecreaseStock(UpdateStockDto dto, CancellationToken token);
    }
}