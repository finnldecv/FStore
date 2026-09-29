namespace FStore.Basket.Domain.Entities;

public class ShoppingCart
{
  public Guid UserId { get; set; }
  public List<ShoppingCartItem> Items { get; set; } = new();
}

public class ShoppingCartItem
{
  public Guid ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public int Quantity { get; set; }
}