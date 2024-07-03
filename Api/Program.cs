using System.Text.Json.Serialization;
using Api;
using Api.Data;
using Api.Data.Seeding;
using Api.Errors;
using Api.Filters;
using Api.Request;
using Api.Validators;
using FluentValidation;
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

// add custom error type to the builder pipeline
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = (context) =>
    {
        var accountErrorFeature = context.HttpContext.Features.Get<AccountErrorFeature>();

        if (accountErrorFeature is not null)
        {
            (string Detail, string Type) details = accountErrorFeature.AccountError switch
            {
                AccountErrorType.InsufficientFundsError => ("The account has insufficient funds.", "https://en.wikipedia.org/wiki/Dishonoured_cheque"),
                _ => ("Account Error", "Account Error")
            };

            context.ProblemDetails.Type = details.Type;
            context.ProblemDetails.Title = "Insufficient Funds";
            context.ProblemDetails.Detail = details.Detail;
        }
    }
);   

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

// migrate to the latest schema for the database context
db.Database.Migrate();

// seed the database if customers table is empty
var seeder = new AccountDbSeeder(db);
seeder.SeedDatabase();

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