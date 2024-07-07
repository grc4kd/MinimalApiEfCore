using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Accounts;
using Infrastructure;
using Infrastructure.Seeding;
using Api;
using Api.Filters;
using Api.Responses;
using Domain.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AccountUser>()
    .AddEntityFrameworkStores<AccountDbContext>();

var connectionString = builder.Configuration.GetConnectionString("AccountsConnection") ?? throw new InvalidOperationException("Connection string 'AccountsConnection' not found.");

builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer(connectionString,
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
    MinInitialDepositAmount = 100,
    MaxDepositAmount = 1_000_000,
    CurrencyUnitScale = 2,
    MaxWithdrawalAmount = 1_000_000
};

builder.Services.AddSingleton(settings);
builder.Services.AddScoped<AccountTransactionActionFilterService>();
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

app.MapIdentityApi<AccountUser>();

// secure Swagger UI endpoints
// app.MapSwagger().RequireAuthorization();

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
    var customers = await db.Customers.AsNoTracking()
        .OrderBy(c => c.Name)
        .OrderBy(c => c.Id)
        .Skip(5 * page)
        .Take(5)
        .Select(c => new GetCustomerResponse(c.Id, c.Name))
        .ToListAsync();

    return TypedResults.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi()
.RequireAuthorization();

customer.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var customerAccounts = await db.Customers
        .AsNoTracking()
        .Include(c => c.Accounts)
        .Where(c => c.Id == id)
        .OrderBy(c => c.Id)
        .Select(c => new GetCustomerResponse(c.Id, c.Name, c.Accounts))
        .ToListAsync();

    if (customerAccounts.Count > 0)
    {
        return TypedResults.Ok(customerAccounts);
    }

    return Results.NotFound();
})
.WithName("GetCustomer")
.WithOpenApi()
.RequireAuthorization();

var account = app.MapGroup("/account");

account.MapGet("/", async (AccountDbContext db, int page = 0) =>
{
    var accounts = await db.Accounts
        .AsNoTracking()
        .OrderBy(a => a.Id)
        .OrderBy(a => a.Customer.Name)
        .Skip(page * 5)
        .Take(5)
        .Select(a => new GetAccountResponse(
            a.Id,
            a.CustomerId,
            a.Customer.Name,
            a.AccountStatus,
            a.AccountType,
            a.Balance
            ))
        .ToListAsync();
    return TypedResults.Ok(accounts);
})
.WithName("GetAccounts")
.WithOpenApi()
.RequireAuthorization();

account.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var account = await db.Accounts
        .AsNoTracking()
        .Include(a => a.Customer)
        .OrderBy(a => a.Id)
        .Where(a => a.Id == id)
        .SingleOrDefaultAsync();

    if (account != null)
    {
        return TypedResults.Ok(new GetAccountResponse
        (
            account.Id,
            account.CustomerId,
            account.Customer.Name,
            account.AccountStatus,
            account.AccountType,
            account.Balance
        ));
    }

    return Results.NotFound();
})
.WithName("GetAccount")
.WithOpenApi()
.RequireAuthorization();

app.Run();

public partial class Program { }

