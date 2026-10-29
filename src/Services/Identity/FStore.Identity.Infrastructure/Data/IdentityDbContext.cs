using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FStore.Identity.Infrastructure.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

  public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
}