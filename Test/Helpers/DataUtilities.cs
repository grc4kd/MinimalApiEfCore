using Domain.Accounts.Data;
using Infrastructure;
using Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

namespace Test.Helpers;

public class DataUtilities
{
    public static async Task ReinitializeDbForTestsAsync(AccountDbContext context)
    {
        await context.Set<Customer>()
            .Include(c => c.Accounts)
            .ExecuteDeleteAsync();

        await AccountDbSeeder.SeedDatabaseAsync(context);
    }
}