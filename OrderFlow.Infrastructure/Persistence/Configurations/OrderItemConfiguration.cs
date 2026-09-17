using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnName("id");

        builder.Property(item => item.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasColumnName("unit_price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(item => item.Total);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}