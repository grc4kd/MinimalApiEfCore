using System.Text.Json.Serialization;
using Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain.Accounts;
using Infrastructure.Seeding;
using Api;
using Api.Filters;

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

customer.MapGet("/", async (AccountDbContext db) =>
{
    var customers = await db.Customers
        .AsNoTracking()
        .Include(c => c.Accounts)
        .Select(c => new { c.Id, c.Name })
        .ToListAsync();
    return TypedResults.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi();

customer.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var customer = await db.Customers
        .AsNoTracking()
        .Include(c => c.Accounts)
        .Where(c => c.Id == id)
        .Select(c => new {c.Id, c.Name,
        Accounts = c.Accounts.Select(a => new {
            a.Id,
            AccountStatus = a.AccountStatus.AccountStatusType.ToString(),
            AccountType = a.AccountType.ToString(),
            a.Balance
        })})
        .FirstOrDefaultAsync();

    if (customer != null)
    {
        return TypedResults.Ok(customer);
    }

    return Results.NotFound();
});

var account = app.MapGroup("/account");

account.MapGet("/", async (AccountDbContext db) =>
{
    var accounts = await db.Accounts
        .AsNoTracking()
        .Include(a => a.Customer)
        .ToListAsync();
    return TypedResults.Ok(accounts);
})
.WithName("GetAccounts")
.WithOpenApi();

account.MapGet("/{id}", async (AccountDbContext db, int id) =>
{
    var account = await db.Accounts
        .Include(a => a.Customer)
        .SingleOrDefaultAsync(a => a.Id == id);

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

