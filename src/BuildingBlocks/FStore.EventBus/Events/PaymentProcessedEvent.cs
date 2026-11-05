namespace FStore.EventBus.Events;

public record PaymentProcessedEvent
{
  public Guid OrderId { get; init; }
  public bool IsSuccessful { get; init; }
  public string? FailureReason { get; init; }
}