using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Product.Api.Infrastructure.Entities;

namespace OrderManagement.Product.Api.Infrastructure.Configuration;

public sealed class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> b)
    {
        b.ToTable("products");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        b.Property(x => x.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        b.Property(x => x.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .IsRequired();

        b.Property(x => x.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("datetime(6)");

        b.Property(x => x.Stock)
             .HasColumnName("stock")
             .IsRequired();

        b.HasIndex(x => x.CategoryId)
            .HasDatabaseName("ix_products_category_id");

        b.HasIndex(x => x.Sku)
            .IsUnique()
            .HasDatabaseName("ux_products_sku");
    }
}