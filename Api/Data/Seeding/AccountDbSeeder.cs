using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeding;

public class AccountDbSeeder
{
    public static int MaxSeededAccountId { get; private set; }
    public static int MaxSeededCustomerId { get; private set; }

    public static async Task SeedDatabaseAsync(AccountDbContext db) {
        // seed the database only if the customer table is empty
        if (!await db.Customers.AnyAsync())
        {
            await db.AddRangeAsync(GetAccountSeedData());
            await db.SaveChangesAsync();
        }

        MaxSeededAccountId = await db.Accounts.MaxAsync(a => a.Id);
        MaxSeededCustomerId = await db.Customers.MaxAsync(c => c.Id);
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
                Id = index,
                Balance = 100,
                Customer = new Customer {
                    Id = index,
                    Name = names[Random.Shared.Next(names.Length)]
                }
            }
        );
    }
}
