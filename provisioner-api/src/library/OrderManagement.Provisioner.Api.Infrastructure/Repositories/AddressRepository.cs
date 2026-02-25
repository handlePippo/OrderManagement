using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Persistence.Configuration;
using OrderManagement.Provisioner.Api.Persistence.Entities;

namespace OrderManagement.Provisioner.Api.Persistence.Repositories;

/// <summary>
/// Address repository.
/// </summary>
public sealed class AddressRepository : IAddressRepository
{
    private readonly UserDbContext DbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="currentUserProvider"></param>
    /// <param name="mapper"></param>
    public AddressRepository(UserDbContext dbContext, ICurrentUserProvider currentUserProvider, IMapper mapper)
    {
        DbContext = dbContext;
        _currentUserProvider = currentUserProvider;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Address>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AddressEntity> entities;
        if (_currentUserProvider.IsAdmin)
        {
            entities = await DbContext
                                      .Addresses
                                       .AsNoTracking()
                                       .ToListAsync(cancellationToken);
        }
        else
        {
            entities = await DbContext
                                      .Addresses
                                      .AsNoTracking()
                                      .Where(a => a.UserId == CurrentUserId)
                                      .ToListAsync(cancellationToken);
        }

        return _mapper.Map<IReadOnlyList<Address>>(entities);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return DbContext
                .Addresses
                .AsNoTracking()
                .AnyAsync(e => e.Id == id, ct);
    }

    public async Task<Address?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Addresses
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity is null ? null : _mapper.Map<Address>(entity);
    }

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<AddressEntity>(address);

        await DbContext
                .Addresses
                .AddAsync(entity, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Address address, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Addresses
                            .FirstOrDefaultAsync(x => x.Id == address.Id && x.UserId == address.UserId, cancellationToken)
                            ?? throw new InvalidOperationException($"Address with id {address.Id} not found.");

        _mapper.Map(address, entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext
                            .Addresses
                            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                            ?? throw new InvalidOperationException($"Address with id {id} not found.");

        DbContext.Remove(entity);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
