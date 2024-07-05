using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeding;

public class AccountDbSeeder
{
    public static async Task SeedDatabaseAsync(AccountDbContext db)
    {
        // seed the database only if the customer table is empty
        if (!await db.Customers.AnyAsync())
        {
            await db.AddRangeAsync(GetAccountSeedData());
            await db.SaveChangesAsync();
        }
    }

    private static IEnumerable<Account> GetAccountSeedData()
    {
        string[] names =
        [
            "Jack", "Jill", "Fred", "Tom", "Harry", "George", "Suzan", "Margerie", "Jolene", "Kate"
        ];

        return Enumerable.Range(1, 5).Select(index =>
            new Account
            {
                AccountType = AccountType.Savings,
                AccountStatus = new AccountStatus(AccountStatusType.OPEN),
                Balance = 100,
                Customer = new Customer
                {
                    Name = names[Random.Shared.Next(names.Length)]
                }
            }
        );
    }
}
