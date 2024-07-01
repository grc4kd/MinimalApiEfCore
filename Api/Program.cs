using Api.Data;
using Api.Request;
using Api.Responses;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseSqlite("DataSource=Accounts.db");
});

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

var names = new[]
{
    "Jack", "Jill", "Fred", "Tom", "Harry", "George", "Suzan", "Margerie", "Jolene", "Kate"
};

var customers = Enumerable.Range(1, 5).Select(index =>
    new Customer {
        Id = index,
        Name = names[Random.Shared.Next(names.Length)]
    }).ToArray();

var accounts = Array.CreateInstance(typeof(Account), customers.Count());
for (int i = 0; i < accounts.Length; i++)
{
    accounts.SetValue(new Account {
        Id = i,
        Customer = customers[i],
        CustomerId = customers[i].Id
    }, i);
}

app.MapGet("/customer", () =>
{
    return customers;
})
.WithName("GetCustomer")
.WithOpenApi();

var account = app.MapGroup("/account");

account.MapGet("/", () =>
{
    return accounts;
})
.WithName("GetAccounts")
.WithOpenApi();

account.MapGet("/{id}", (int id) =>
{
    var account = new Account {
        Id = id,
        CustomerId = Random.Shared.Next(1, 5),
        Customer = new Customer {
            Name = names[Random.Shared.Next(names.Length)]
        },
    };
    return TypedResults.Created($"/account/{account.Id}", account);
})
.WithName("GetAccount")
.WithOpenApi();

account.MapPost("/deposit", (DepositRequest deposit) => {
    var balance = deposit.Amount;

    return TypedResults.Created($"/account/{deposit.AccountId}", 
        new DepositResponse(deposit.CustomerId, deposit.AccountId, balance, true));
})
.WithName("Deposit")
.WithOpenApi();

account.MapPost("/withdrawal", (WithdrawalRequest withdrawal) => {
    var balance = withdrawal.Amount;

    return TypedResults.Created($"/account/{withdrawal.AccountId}", 
        new WithdrawalResponse(withdrawal.CustomerId, withdrawal.AccountId, balance, true));
})
.WithName("Withdrawal")
.WithOpenApi();

account.MapPut("/close", (CloseAccountRequest closeAccount) => {
    var account = new Account {
        Id = closeAccount.AccountId,
        CustomerId = closeAccount.CustomerId,
        Customer = new Customer {
            Id = closeAccount.CustomerId,
            Name = names[Random.Shared.Next(names.Length)]
        }
    };

    return new CloseAccountResponse(account.CustomerId, account.Id, true);
})
.WithName("CloseAccount")
.WithOpenApi();

app.Run();

public partial class Program { }