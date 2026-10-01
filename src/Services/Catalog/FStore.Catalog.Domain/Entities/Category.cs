using System.Net.Http.Headers;
using FStore.Catalog.Domain.Common;

namespace FStore.Catalog.Domain.Entities;

public class Category : BaseEntity
{
  public string Name { get; private set; } = string.Empty;
  public ICollection<Product> Products { get; private set; } = new List<Product>();

  private Category() { }

  public Category(string name)
  {
    Id = Guid.NewGuid();
    Name = name;
  }

  public Category(Guid id, string name)
  {
    Id = id;
    Name = name;
  }
  public void Update(string name)
  {
    Name = name;
  }
}