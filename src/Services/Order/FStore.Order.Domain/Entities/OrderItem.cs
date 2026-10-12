using FStore.Order.Domain.Common;

namespace FStore.Order.Domain.Entities;

public class OrderItem : BaseEntity
{
  public Guid ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public decimal UnitPrice { get; set; }
  public int Quantity { get; set; }

  public decimal LineTotal => UnitPrice * Quantity;

  public Guid OrderId { get; set; }
  public Order? Order { get; set; }
}