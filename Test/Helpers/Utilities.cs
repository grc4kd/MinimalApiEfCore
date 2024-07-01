using Api.Data;

namespace Test.Helpers;

public static class Utilities
{
    internal static int MaxDbCustomerId => GetSeedingAccounts().Max(a => a.Customer.Id);
    internal static int MaxDbAccountId => GetSeedingAccounts().Max(a => a.Id);

    public static async Task InitializeDbForTests(AccountDbContext db) 
    {
        await db.Database.EnsureCreatedAsync();
        await db.Accounts.AddRangeAsync(GetSeedingAccounts());
        await db.SaveChangesAsync();
    }

    public static async Task ReinitializeDbForTests(AccountDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        db.Customers.RemoveRange(db.Customers);
        await db.SaveChangesAsync();
        await InitializeDbForTests(db);
    }

    public static List<Account> GetSeedingAccounts()
    {
        return 
        [
            new Account() {
                Id = 1,
                Customer = new Customer {
                    Id = 1,
                    Name = "Ada"
                }
            },
            new Account() {
                Id = 2,
                Customer = new Customer {
                    Id = 2,
                    Name = "Barbara"
                },
                Balance = 200
            },
            new Account() {
                Id = 3,
                Customer = new Customer {
                    Id = 3,
                    Name = "Grace"
                }
            }
        ];
    }
}