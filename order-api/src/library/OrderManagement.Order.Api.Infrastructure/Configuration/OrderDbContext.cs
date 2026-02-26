using Microsoft.EntityFrameworkCore;
using OrderManagement.Order.Api.Infrastructure.Entities;

namespace OrderManagement.Order.Api.Infrastructure.Configuration;

public class OrderDbContext : DbContext
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
}
