namespace OrderManagement.Order.Api.Application.Interfaces;

public interface IUnitOfWork
{
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken token = default);
    Task SaveChangesAsync(CancellationToken token = default);
    Task CommitAsync(CancellationToken token = default);
    Task RollbackAsync(CancellationToken token = default);
}