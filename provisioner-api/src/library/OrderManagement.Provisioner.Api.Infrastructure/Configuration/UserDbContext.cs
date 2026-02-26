using Microsoft.EntityFrameworkCore;
using OrderManagement.Provisioner.Api.Infrastructure.Entities;

namespace OrderManagement.Provisioner.Api.Infrastructure.Configuration;

public class UserDbContext : DbContext
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AddressEntity> Addresses => Set<AddressEntity>();

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
}
