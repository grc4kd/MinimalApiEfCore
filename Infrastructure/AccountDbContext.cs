using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .Property(t => t.Balance)
            .HasColumnType("decimal")
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Account>().OwnsOne(p => p.AccountStatus);
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
}