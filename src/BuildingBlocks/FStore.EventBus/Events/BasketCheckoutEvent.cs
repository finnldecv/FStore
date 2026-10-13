namespace FStore.EventBus.Events;

public record BasketCheckoutEvent
{
  public Guid UserId { get; init; }
  public string ShippingAddress { get; init; } = string.Empty;
  public string? Notes { get; init; }
  public List<BasketCheckoutItem> Items { get; init; } = new();
}

public record BasketCheckoutItem
{
  public Guid ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public decimal UnitPrice { get; init; }
  public int Quantity { get; init; }
}