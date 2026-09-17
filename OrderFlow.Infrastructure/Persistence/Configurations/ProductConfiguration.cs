using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasColumnName("id");

        builder.Property(product => product.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(product => product.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.Stock)
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(product => product.Version)
    .HasColumnName("xmin")
    .HasColumnType("xid")
    .ValueGeneratedOnAddOrUpdate()
    .IsConcurrencyToken();
    }
}