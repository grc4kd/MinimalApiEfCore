using Microsoft.EntityFrameworkCore;

namespace Api.Data.Seeding;

public class AccountDbSeeder(AccountDbContext db)
{
    public int MaxSeededAccountId { get; private set; }
    public int MaxSeededCustomerId { get; private set; }

    private readonly AccountDbContext _db = db;

    public void SeedDatabase() {
        string[] names =
        [
            "Jack", "Jill", "Fred", "Tom", "Harry", "George", "Suzan", "Margerie", "Jolene", "Kate"
        ];

        // seed the database if customers table is empty
        if (!_db.Customers.Any())
        {
            var customers = Enumerable.Range(1, 5).Select(index =>
                new Customer
                {
                    Id = index,
                    Name = names[Random.Shared.Next(names.Length)]
                });

            var accounts = new List<Account>();
            foreach (var customer in customers) {
                accounts.Add(new Account {
                    Customer = customer
                });
            }

            _db.AddRange(accounts);
            _db.SaveChanges();
        }

        MaxSeededAccountId = _db.Accounts.Max(a => a.Id);
        MaxSeededCustomerId = _db.Customers.Max(c => c.Id);
    }
}
