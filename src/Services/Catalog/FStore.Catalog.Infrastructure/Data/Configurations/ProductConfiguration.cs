using FStore.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FStore.Catalog.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.HasKey(p => p.Id);

    builder.Property(p => p.Name)
    .IsRequired()
    .HasMaxLength(200);

    builder.Property(p => p.Description)
    .HasMaxLength(2000);

    builder.OwnsOne(p => p.Price, money =>
    {
      money.Property(m => m.Amount)
      .HasColumnName("Price")
      .HasColumnType("decimal(18,2)")
      .IsRequired();
      
      money.Property(m => m.Currency)
      .HasColumnName("Currency")
      .HasMaxLength(3)
      .IsRequired();
    });
  }
}