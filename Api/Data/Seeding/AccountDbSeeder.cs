using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeding;

public class AccountDbSeeder(AccountDbContext db) : IDisposable, IAsyncDisposable
{
    public int MaxSeededAccountId { get; private set; }
    public int MaxSeededCustomerId { get; private set; }

    private readonly AccountDbContext _db = db;

    public void SeedDatabase() {
        // seed the database only if the customers table is empty
        if (!_db.Customers.Any())
        {
            _db.AddRange(GetAccountSeedData());
            _db.SaveChanges();
        }

        MaxSeededAccountId = _db.Accounts.Max(a => a.Id);
        MaxSeededCustomerId = _db.Customers.Max(c => c.Id);
    }

    public async Task SeedDatabaseAsync() {
        // seed the database only if the customer table is empty
        if (!await _db.Customers.AnyAsync())
        {
            await _db.AddRangeAsync(GetAccountSeedData());
            await _db.SaveChangesAsync();
        }

        MaxSeededAccountId = await _db.Accounts.MaxAsync(a => a.Id);
        MaxSeededCustomerId = await _db.Customers.MaxAsync(c => c.Id);
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _db.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _db.DisposeAsync();
    }
}
