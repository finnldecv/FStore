using FStore.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Infrastructure.Data;

public class CatalogDbContext : DbContext
{
  public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
  
  public DbSet<Product> Products => Set<Product>();
  public DbSet<Category> Categories => Set<Category>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }
}