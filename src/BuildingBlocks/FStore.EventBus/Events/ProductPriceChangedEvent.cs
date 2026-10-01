namespace FStore.EventBus.Events;

public record ProductPriceChangedEvent
{
  public Guid ProductId {get; init;}
  public string ProductName {get; init;} = string.Empty;
  public decimal OldPrice {get; init;}
  public decimal NewPrice {get; init;}
  public DateTime OccurredAt {get; init;} = DateTime.UtcNow;
}