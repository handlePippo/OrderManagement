using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Order.Api.Infrastructure.Entities;

namespace OrderManagement.Order.Api.Infrastructure.Configuration;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> b)
    {
        b.ToTable("order_items");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        b.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        b.Property(x => x.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.UnitPrice)
            .HasColumnName("unit_price")
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

        b.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_order_items_product_id");

        b.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_order_items_order_id");

        b.HasOne<OrderEntity>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}