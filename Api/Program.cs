using System.Text.Json.Serialization;
using Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Accounts;
using Infrastructure.Seeding;
using Api;
using Api.Filters;
using Domain.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:AccountsConnection",
        b => b.MigrationsAssembly("Api")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // prevent serialization cycles for minimal API endpoints
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// get system settings from configuration during application startup
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

Settings? settings = config.GetSection("Settings").Get<Settings>();

// default settings when required section settings is not found
settings ??= new Settings
{
    MinInitialDepositAmount = 100.00m,
    MaxDepositAmount = 1_000_000.00m,
    MaxWithdrawalAmount = 1_000_000.00m
};

builder.Services.AddSingleton(settings);
builder.Services.AddScoped<CurrencyActionFilterService>();
builder.Services.AddScoped<AccountActionFilterService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // do not serialize cyclical references found in the object graph of entities
        // to prevent cycles after fix-up of entity navigation properties
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await using var scope = app.Services.CreateAsyncScope();
await using var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

// seed the database if customers table is empty
await AccountDbSeeder.SeedDatabaseAsync(db);

var customer = app.MapGroup("/customer");

customer.MapGet("/", async (AccountDbContext db, int page = 0) =>
{
    var customers = await db.Customers
        .AsNoTracking()
        .Select(c => new
        {
            CustomerId = c.Id,
            c.Name
        })
        .OrderBy(c => c.Name)
        .OrderBy(c => c.CustomerId)
        .Skip(5 * page)
        .Take(5)
        .OrderBy(c => c.CustomerId)
        .OrderBy(c => c.Name)
        .ToListAsync();
    return TypedResults.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi();

customer.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var customerAccounts = await db.Customers
        .AsNoTracking()
        .Include(c => c.Accounts)
        .Where(c => c.Id == id)
        .Select(c => new
        {
            CustomerId = c.Id,
            c.Name,
            Accounts = c.Accounts.Select(a => new
            {
                AccountId = a.Id,
                AccountStatus = a.AccountStatus.AccountStatusType.ToString(),
                AccountType = a.AccountType.ToString(),
                a.Balance
            })
        })
        .OrderBy(c => c.CustomerId)
        .ToListAsync();

    if (customerAccounts.Count > 0)
    {
        return TypedResults.Ok(customerAccounts);
    }

    return Results.NotFound();
})
.WithName("GetCustomer")
.WithOpenApi();

var account = app.MapGroup("/account");

account.MapGet("/", async (AccountDbContext db, int page = 0) =>
{
    var accounts = await db.Accounts
        .AsNoTracking()
        .Select(a => new
        {
            AccountId = a.Id,
            a.CustomerId,
            a.Customer.Name,
            AccountStatus = a.AccountStatus.AccountStatusType.ToString(),
            AccountType = a.AccountType.ToString(),
            a.Balance
        })
        .OrderBy(a => a.AccountId)
        .OrderBy(a => a.Name)
        .Skip(page * 5)
        .Take(5)
        .ToListAsync();
    return TypedResults.Ok(accounts);
})
.WithName("GetAccounts")
.WithOpenApi();

account.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var account = await db.Accounts
        .AsNoTracking()
        .Include(a => a.Customer)
        .Select(a => new
        {
            AccountId = a.Id,
            a.CustomerId,
            a.Customer.Name,
            AccountStatus = a.AccountStatus.AccountStatusType.ToString(),
            AccountType = a.AccountType.ToString(),
            a.Balance
        })
        .OrderBy(a => a.AccountId)
        .SingleOrDefaultAsync(a => a.AccountId == id);

    if (account != null)
    {
        return TypedResults.Ok(account);
    }

    return Results.NotFound();
})
.WithName("GetAccount")
.WithOpenApi();

app.Run();

public partial class Program { }

