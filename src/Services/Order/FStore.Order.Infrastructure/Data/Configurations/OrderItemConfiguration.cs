using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FStore.Order.Domain.Entities;

namespace FStore.Order.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
  public void Configure(EntityTypeBuilder<OrderItem> builder)
  {
    builder.ToTable("OrderItem");
    builder.HasKey(i => i.Id);

    builder.Property(i => i.ProductId).IsRequired();
    builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
    builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
    builder.Property(i => i.Quantity).IsRequired();

    builder.Ignore(i => i.LineTotal);
  }
}