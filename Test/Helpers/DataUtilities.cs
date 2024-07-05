using Infrastructure;
using Infrastructure.Seeding;

namespace Test.Helpers;

public class DataUtilities
{
    public static async Task ReinitializeDbForTestsAsync(AccountDbContext context)
    {
        context.RemoveRange(context.Customers);
        await context.SaveChangesAsync();

        await AccountDbSeeder.SeedDatabaseAsync(context);
    }
}