namespace FStore.Payment.Domain.Entities;

public class PaymentRecord
{
  public Guid Id {get; set;} = Guid.NewGuid();
  public Guid OrderId{get;set;}
  public Guid UserId {get;set;}
  public decimal Amount {get;set;}
  public string Status {get;set;} = "Pending";
  public string? FailureReason{get;set;}
  public DateTime ProcessedAt {get;set;} = DateTime.UtcNow;
}