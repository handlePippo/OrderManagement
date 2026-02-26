using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Order.Api.Infrastructure.Entities;

namespace OrderManagement.Order.Api.Infrastructure.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> b)
    {
        b.ToTable("orders");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedNever();

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.SubTotal)
            .HasColumnName("subtotal")
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Total)
            .HasColumnName("total")
            .HasPrecision(18, 2)
            .IsRequired();

        b.OwnsOne(x => x.ShippingAddress, sa =>
        {
            sa.Ignore(x => x.Id);

            sa.Ignore(x => x.UserId);

            sa.Property(x => x.ShipAddress)
                .HasColumnName("ship_address")
                .HasMaxLength(100)
                .IsRequired();

            sa.Property(x => x.ShipCity)
                .HasColumnName("ship_city")
                .HasMaxLength(50)
                .IsRequired();

            sa.Property(x => x.ShipPostalCode)
                .HasColumnName("ship_postal_code")
                .HasMaxLength(20)
                .IsRequired();

            sa.Property(x => x.ShipCountryCode)
                .HasColumnName("ship_country_code")
                .HasMaxLength(2)
                .IsRequired();

            sa.Property(x => x.ShipPhoneNumber)
                .HasColumnName("ship_phone_number")
                .HasMaxLength(20)
                .IsRequired();

            sa.WithOwner();
        });

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .IsRequired();

        b.Property(x => x.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("datetime(6)");

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_orders_user_id");

        b.Navigation(x => x.ShippingAddress).IsRequired();
    }
}
