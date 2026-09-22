namespace FStore.EventBus;

public abstract class IntergrationEvent : IIntergrationEvent
{
  public Guid Id { get; } = Guid.NewGuid();

  public DateTime OccurredOn { get; } = DateTime.UtcNow;
}