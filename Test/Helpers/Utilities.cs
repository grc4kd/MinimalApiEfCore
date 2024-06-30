using Api.Data;

namespace Test.Helpers;

public static class Utilities
{
    public static void InitializeDbForTests(AccountDbContext db) 
    {
        db.Customers.AddRange(GetSeedingCustomers());
        db.Accounts.AddRange(GetSeedingAccounts(db.Customers));
    }

    public static void ReinitializeDbForTests(AccountDbContext db)
    {
        db.Accounts.RemoveRange(db.Accounts);
        db.Customers.RemoveRange(db.Customers);
        InitializeDbForTests(db);
    }

    public static List<Customer> GetSeedingCustomers()
    {
        return
        [
            new Customer() { Name = "Arnold" },
            new Customer() { Name = "Benjamin" },
            new Customer() { Name = "Charles" }
        ];
    }

    public static List<Account> GetSeedingAccounts(IEnumerable<Customer> customers)
    {
        var accounts = new List<Account>();
        foreach (var customer in customers)
        {
            accounts.Add(new Account() {
                Customer = customer
            });
        }
        return accounts;
    }
}