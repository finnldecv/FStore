using FStore.Catalog.Domain.Common;
using FStore.Catalog.Domain.ValueObjects;

namespace FStore.Catalog.Domain.Entities;

public class Product : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Money Price { get; set; } = Money.Usd(0);
  public int StockQuantity { get; set; }
  public Guid CategoryId { get; set; }
  public Category? Category { get; set; }
}