using FStore.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FStore.Order.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
  public void Configure(EntityTypeBuilder<OrderEntity> builder)
  {
    builder.ToTable("Orders");
    builder.HasKey(o => o.Id);

    builder.Property(o => o.UserId).IsRequired();
    builder.Property(o => o.Status).HasConversion<int>().IsRequired();
    builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
    builder.Property(o => o.Notes).HasMaxLength(2000);
    builder.Property(o => o.CancellationReason).HasMaxLength(500);

    builder.HasMany(o => o.Items)
      .WithOne(i => i.Order!)
      .HasForeignKey(i => i.OrderId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.Ignore(o => o.TotalAmount);
    builder.Ignore(o => o.UpdatedAt);
  }
}