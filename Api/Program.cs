using System.Net.Http.Headers;
using Api.Data;
using Api.Request;
using Api.Responses;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/account", () =>
{
    return accounts;
})
.WithName("GetAccounts")
.WithOpenApi();

app.MapGet("/account/{id}", (int id) =>
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

app.MapPost("/account/open", (OpenAccount openAccount) => {
    var account = new Account {
        Id = 1,
        CustomerId = openAccount.CustomerId,
        Customer = new Customer {
            Id = openAccount.CustomerId,
            Name = names[Random.Shared.Next(names.Length)]
        }
    };

    return TypedResults.Created($"/account/{account.Id}", 
        new OpenAccountResponse(account.CustomerId, account.Id, true));
})
.WithName("OpenAccount")
.WithOpenApi();

app.MapPost("/account/deposit", (Deposit deposit) => {
    var balance = deposit.Amount;

    return TypedResults.Created($"/account/{deposit.AccountId}", 
        new DepositResponse(deposit.CustomerId, deposit.AccountId, balance, true));
})
.WithName("Deposit")
.WithOpenApi();

app.MapPost("/account/withdrawal", (Withdrawal withdrawal) => {
    var balance = withdrawal.Amount;

    return TypedResults.Created($"/account/{withdrawal.AccountId}", 
        new WithdrawalResponse(withdrawal.CustomerId, withdrawal.AccountId, balance, true));
})
.WithName("Withdrawal")
.WithOpenApi();

app.MapPut("/account/close", (CloseAccount closeAccount) => {
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