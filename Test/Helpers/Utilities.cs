using Api.Data;

namespace Test.Helpers;

public static class Utilities
{
    internal static int MaxDbCustomerId => GetSeedingAccounts().Max(a => a.Customer.Id);

    public static void InitializeDbForTests(AccountDbContext db) 
    {
        db.Database.EnsureCreated();
        db.Accounts.AddRange(GetSeedingAccounts());
        db.SaveChanges();
    }

    public static void ReinitializeDbForTests(AccountDbContext db)
    {
        db.Customers.RemoveRange(db.Customers);
        db.SaveChanges();
        InitializeDbForTests(db);
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
                }
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