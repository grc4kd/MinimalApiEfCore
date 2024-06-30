using Api.Data;
using Api.Request;
using Api.Responses;

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

var summaries = new[]
{
    "Jack", "Jill", "Fred", "Tom", "Harry", "George", "Suzan", "Margerie", "Jolene", "Kate"
};

app.MapGet("/customer", () =>
{
    var customers = Enumerable.Range(1, 5).Select(index =>
        new Customer {
            Id = index,
            Name = summaries[Random.Shared.Next(summaries.Length)]
        })
        .ToArray();
    return customers;
})
.WithName("GetCustomer")
.WithOpenApi();

app.MapGet("/account", () =>
{
    var accounts = Enumerable.Range(1, 5).Select(index =>
        new Account {
            Id = index,
            CustomerId = index,
            Customer = new Customer {
                Id = index,
                Name = summaries[Random.Shared.Next(summaries.Length)]
            }         
        })
        .ToArray();
    return accounts;
})
.WithName("GetAccount")
.WithOpenApi();

app.MapPost("/account/open", (OpenAccount openAccount) => {
    return TypedResults.Created("/account", new OpenAccountResponse{
        CustomerId = openAccount.CustomerId,
        AccountId = 1,
        Succeeded = true
    });
})
.WithName("OpenAccount")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program { }