using FStore.Order.Domain.Common;
using FStore.Order.Domain.Enums;

namespace FStore.Order.Domain.Entities;

public class Order : BaseEntity
{
  public Guid UserId;
  public OrderStatus Status { get; private set; } = OrderStatus.Pending;
  public string? CancellationReason { get; private set; }

  public string ShippingAddress { get; set; } = string.Empty;
  public string? Notes { get; set; }

  public readonly List<OrderItem> _items = new();
  public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

  public decimal TotalAmount => _items.Sum(i => i.LineTotal);

  public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
  {
    if (quantity <= 0)
    {
      throw new ArgumentException("Quantity must be positive", nameof(quantity));
    }

    if (unitPrice < 0)
    {
      throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
    }

    _items.Add(new OrderItem
    {
      ProductId = productId,
      ProductName = productName,
      UnitPrice = unitPrice,
      Quantity = quantity,
      OrderId = Id
    });
  }

  public void MarkAsConfirmed()
  {
    if (Status != OrderStatus.Pending)
    {
      throw new InvalidOperationException(
        $"Cannot confirm an order in status {Status}."
      );
    }

    Status = OrderStatus.Confirmed;
    UpdatedAt = DateTime.UtcNow;
  }

  public void MarkAsCancelled(string reason)
  {
    if (Status == OrderStatus.Cancelled)
    {
      throw new InvalidOperationException("Order is already cancelled.");
    }

    if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
    {
      throw new InvalidOperationException(
        $"Cannot cancel an order that has been {Status}");
    }

    Status = OrderStatus.Cancelled;
    CancellationReason = reason;
    UpdatedAt = DateTime.UtcNow;
  }

  public void MarkAsShipped()
  {
    if (Status != OrderStatus.Confirmed)
    {
      throw new InvalidOperationException(
        $"Cannot ship an order in status {Status}"
      );
    }

    Status = OrderStatus.Shipped;
    UpdatedAt = DateTime.UtcNow;
  }

  public void MarkAsDelivered()
  {
    if (Status != OrderStatus.Shipped)
    {
      throw new InvalidOperationException(
        $"Cannot deliver an order in status {Status}"
      );
    }

    Status = OrderStatus.Delivered;
    UpdatedAt = DateTime.UtcNow;
  }
}