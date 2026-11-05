using FStore.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FStore.Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
  public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

  public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
}