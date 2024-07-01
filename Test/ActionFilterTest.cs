using System.Net;
using Api.Data;
using Api.Request;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Helpers;

namespace Test;

public class ActionFilterTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ActionFilterTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AccountDbContext>();

        await Utilities.ReinitializeDbForTests(db);
    }

    [Fact]
    public async Task Validate_WithdrawalRequestAmount_DecimalPlaces()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: 1.001m);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Validate_DepositRequestAmount_DecimalPlaces()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: 1, accountId: 1, amount: 1.001m);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);   
    }

    [Fact]
    public async Task Post_DepositWithAmountOverMaximum_BadRequest()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: 1, accountId: 1, decimal.MaxValue);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async void Post_WithdrawalWithAmountOverMaximum_BadRequest()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 1, decimal.MaxValue);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}