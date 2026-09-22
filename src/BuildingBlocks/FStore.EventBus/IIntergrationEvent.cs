namespace FStore.EventBus;

public interface IIntergrationEvent
{
  Guid Id { get; }
  DateTime OccurredOn { get; }
}