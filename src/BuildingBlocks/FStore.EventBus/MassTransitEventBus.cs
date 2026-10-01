using MassTransit;

namespace FStore.EventBus;

public class MassTransitEventBus : IEventBus
{
  private readonly IPublishEndpoint _publishEndpoint;
  public MassTransitEventBus(IPublishEndpoint publishEndpoint)
  {
    _publishEndpoint = publishEndpoint;
  }
  public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
  {
    return _publishEndpoint.Publish(@event, ct);
  }
}