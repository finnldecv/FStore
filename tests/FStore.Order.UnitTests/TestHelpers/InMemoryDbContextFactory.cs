using FStore.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.UnitTests.TestHelpers;

public class InMemoryDbContextFactory
{
  public static OrderDbContext Create()
  {
    var options = new DbContextOptionsBuilder<OrderDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    var db = new OrderDbContext(options);
    db.Database.EnsureCreated();
    return db;
  }
}