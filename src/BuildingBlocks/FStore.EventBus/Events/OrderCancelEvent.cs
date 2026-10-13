namespace FStore.EventBus.Events;

public record OrderCancelledEvent
{
  public Guid OrderId { get; init; }
  public Guid UserId { get; init; }
  public string Reason { get; init; } = string.Empty;
}