using FStore.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.UnitTests.TestHelpers;

public static class InMemoryDbContextFactory
{
  public static CatalogDbContext Create()
  {
    var options = new DbContextOptionsBuilder<CatalogDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    var db = new CatalogDbContext(options);
    db.Database.EnsureCreated();
    return db;
  }
}