using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Category.Api.Infrastructure.Entities;

namespace OrderManagement.Category.Api.Infrastructure.Configuration;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> b)
    {
        b.ToTable("categories");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .IsRequired();

        b.Property(x => x.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("datetime(6)");

        b.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("ux_categories_name");
    }
}