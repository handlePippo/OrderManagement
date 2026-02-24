using Microsoft.EntityFrameworkCore;
using OrderManagement.Product.Api.Infrastructure.Entities;

namespace OrderManagement.Product.Api.Infrastructure.Configuration;

public class ProductDbContext : DbContext
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
}
