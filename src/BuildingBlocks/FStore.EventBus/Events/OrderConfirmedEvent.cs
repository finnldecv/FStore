namespace FStore.EventBus.Events;

public record OrderConfirmedEvent
{
  public Guid OrderId { get; init; }
  public Guid UserId { get; init; }
  public decimal TotalAmount { get; init; }
}