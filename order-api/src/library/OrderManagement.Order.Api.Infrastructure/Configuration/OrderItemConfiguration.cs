using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Order.Api.Persistence.Entities;

namespace OrderManagement.Order.Api.Persistence.Configuration;

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

        b.OwnsOne(x => x.ProductInfo, pi =>
        {
            pi.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            pi.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            pi.Property(x => x.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(200)
                .IsRequired();

            pi.Property(x => x.UnitPrice)
                .HasColumnName("unit_price")
                .HasPrecision(18, 2)
                .IsRequired();

            pi.Property(x => x.LineTotal)
                .HasColumnName("line_total")
                .HasPrecision(18, 2)
                .IsRequired();

            pi.HasIndex(x => x.ProductId)
                .HasDatabaseName("ix_order_items_product_id");

            pi.WithOwner();
        });

        b.Navigation(x => x.ProductInfo).IsRequired();

        b.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_order_items_order_id");

        b.HasOne<OrderEntity>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}