namespace FStore.Order.Domain.Common;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; } = Guid.NewGuid();
  public DateTime CreateAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
}