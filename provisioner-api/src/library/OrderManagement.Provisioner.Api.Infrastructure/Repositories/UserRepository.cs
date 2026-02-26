using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using OrderManagement.Provisioner.Api.Infrastructure.Configuration;
using OrderManagement.Provisioner.Api.Infrastructure.Entities;

namespace OrderManagement.Provisioner.Api.Infrastructure.Repositories;

/// <summary>
/// User repository.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly UserDbContext DbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="mapper"></param>
    public UserRepository(UserDbContext dbContext, IMapper mapper, ICurrentUserProvider currentUserProvider)
    {
        DbContext = dbContext;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<UserEntity> entities;
        if (_currentUserProvider.IsAdmin)
        {
            entities = await DbContext
                .Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        else
        {
            entities = await DbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Id == CurrentUserId)
                .ToListAsync(cancellationToken);
        }

        return _mapper.Map<IReadOnlyList<User>>(entities);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbContext
                .Users
                .AsNoTracking()
                .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<User?> GetAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Email == tokenRequest.Email && e.PasswordHash == tokenRequest.Password, cancellationToken);

        return _mapper.Map<User>(entity);
    }

    public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                        .Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return _mapper.Map<User>(entity);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<UserEntity>(user);

        await DbContext
                .Users
                .AddAsync(entity, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Users
                            .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken)
                            ?? throw new InvalidOperationException($"User with id {user.Id} not found.");

        _mapper.Map(user, entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await DbContext
                                .Users
                                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                                ?? throw new InvalidOperationException($"User with id {id} not found.");

        DbContext.Remove(dbEntity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}