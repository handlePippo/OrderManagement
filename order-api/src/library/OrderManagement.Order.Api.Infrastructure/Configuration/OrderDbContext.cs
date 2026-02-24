using Microsoft.EntityFrameworkCore;
using OrderManagement.Order.Api.Persistence.Entities;

namespace OrderManagement.Order.Api.Persistence.Configuration;

public class OrderDbContext : DbContext
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
}
