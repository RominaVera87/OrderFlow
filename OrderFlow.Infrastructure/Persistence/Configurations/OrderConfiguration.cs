using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(
        EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasColumnName("id");

        builder.Property(order => order.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(order => order.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(order => order.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Ignore(order => order.Total);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}