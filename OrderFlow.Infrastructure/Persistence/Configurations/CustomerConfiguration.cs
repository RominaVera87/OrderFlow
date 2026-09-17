using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration
    : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .HasColumnName("id");

        builder.Property(customer => customer.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasColumnName("email")
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique();
    }
}