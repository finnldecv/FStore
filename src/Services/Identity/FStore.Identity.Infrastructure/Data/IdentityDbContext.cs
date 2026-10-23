using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FStore.Identity.Infrastructure.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
}