using Microsoft.EntityFrameworkCore.Storage;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Infrastructure.Configuration;

namespace OrderManagement.Order.Api.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(ct);
        return _transaction;
    }

    public Task SaveChangesAsync(CancellationToken token = default) => _context.SaveChangesAsync(token);
    public Task CommitAsync(CancellationToken token = default) => _transaction?.CommitAsync(token) ?? Task.CompletedTask;
    public Task RollbackAsync(CancellationToken token = default) => _transaction?.RollbackAsync(token) ?? Task.CompletedTask;
}