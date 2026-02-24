using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Provisioner.Api.Persistence.Entities;

namespace OrderManagement.Provisioner.Api.Persistence.Configuration;

public sealed class AddressConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> b)
    {
        b.ToTable("addresses");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(2)
            .IsRequired();

        b.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(10)
            .IsRequired();

        b.Property(x => x.Street)
            .HasColumnName("street")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .IsRequired();

        b.Property(x => x.ModifiedAt)
            .HasColumnType("datetime(6)")
            .HasColumnName("modified_at");

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_addresses_user_id");

        b.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}