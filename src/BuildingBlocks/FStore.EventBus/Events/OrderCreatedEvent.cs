namespace FStore.EventBus.Events;

public record OrderCreatedEvent
{
  public Guid OrderId { get; init; }
  public Guid UserId { get; init; }
  public decimal TotalAmount { get; init; }
  public List<OrderCreatedItem> Items { get; init; } = new();
}

public record OrderCreatedItem
{
  public Guid ProductId { get; init; }
  public int Quantity { get; init; }
}