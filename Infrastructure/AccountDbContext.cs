using Domain.Accounts.Data;
using Domain.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : IdentityDbContext<AccountUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>()
            .Property(t => t.Balance)
            .HasColumnType("decimal")
            .HasPrecision(18, 2);

        builder.Entity<Account>()
            .OwnsOne(p => p.AccountStatus);
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<AccountUser> AccountUsers { get; set; }
}
