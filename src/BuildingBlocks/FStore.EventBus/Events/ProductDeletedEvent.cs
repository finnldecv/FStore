namespace FStore.EventBus.Events;

public record ProductDeletedEvent
{
  public Guid ProductId {get; init;}
  public DateTime OccuredAt{get; init;} = DateTime.UtcNow;
}