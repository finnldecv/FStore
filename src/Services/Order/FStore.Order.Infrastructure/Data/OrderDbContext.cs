using FStore.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
  public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) {}

  DbSet<OrderEntity> Orders => Set<OrderEntity>();
  DbSet<OrderItem> OrderItems => Set<OrderItem>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }
}