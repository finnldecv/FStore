namespace FStore.EventBus.Events;

public record StockReservationFailedEvent
{
  public Guid OrderId { get; init; }
  public string Reason { get; init; } = string.Empty;
}