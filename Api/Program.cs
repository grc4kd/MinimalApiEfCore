using System.Text.Json.Serialization;
using Api;
using Api.Data;
using Api.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseSqlite("DataSource=Accounts.db");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // do not serialize cyclical references found in the object graph of entities
        // to prevent cycles after fix-up of entity navigation properties
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.ConfigureHttpJsonOptions(options => {
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
settings ??= new Settings { MaxWithdrawalAmount = decimal.MaxValue, MaxDepositAmount = decimal.MaxValue };

CurrencyActionFilter.MaxDepositAmount = settings.MaxDepositAmount;
CurrencyActionFilter.MaxWithdrawalAmount = settings.MaxWithdrawalAmount;

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

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
await db.Database.MigrateAsync();

// seed the database if customers table is empty
if (!await db.Customers.AnyAsync())
{
    var customers = Enumerable.Range(1, 5).Select(index =>
        new Customer {
            Id = index,
            Name = names[Random.Shared.Next(names.Length)]
        });

    await db.AddRangeAsync(customers);
    await db.SaveChangesAsync();
}

var customer = app.MapGroup("/customer");

customer.MapGet("/", async (AccountDbContext db) =>
{
    var customers = await db.Customers
        .AsNoTracking()
        .Include(c => c.Accounts)
        .ToListAsync();
    return TypedResults.Ok(customers);
})
.WithName("GetCustomer")
.WithOpenApi();

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
        return TypedResults.Created($"/account/{account.Id}", account);
    }

    return Results.NotFound();
})
.WithName("GetAccount")
.WithOpenApi();

app.Run();

public partial class Program { }