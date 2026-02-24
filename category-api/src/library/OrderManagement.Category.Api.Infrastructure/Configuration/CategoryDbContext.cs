using Microsoft.EntityFrameworkCore;
using OrderManagement.Category.Api.Infrastructure.Entities;

namespace OrderManagement.Category.Api.Infrastructure.Configuration;

public class CategoryDbContext : DbContext
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryDbContext).Assembly);
}
