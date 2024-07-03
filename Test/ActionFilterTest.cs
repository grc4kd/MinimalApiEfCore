using System.Net;
using Api.Data;
using Api.Request;
using Microsoft.AspNetCore.Mvc.Testing;

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

    [Fact]
    public async Task Validate_WithdrawalRequestAmount_DecimalPlaces()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: 1.001m);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Validate_DepositRequestAmount_DecimalPlaces()
    {
        var request = new DepositRequest(customerId: 1, accountId: 1, amount: 1.001m);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);   
    }

    [Fact]
    public async Task Post_DepositWithAmountOverMaximum_BadRequest()
    {
        var request = new DepositRequest(customerId: 1, accountId: 1, decimal.MaxValue);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async void Post_WithdrawalWithAmountOverMaximum_BadRequest()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 1, decimal.MaxValue);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_OpenAccountWithoutMinimumDeposit_BadRequest()
    {
        var request = new OpenAccountRequest(customerId: 1, accountType: AccountType.Savings, 20);
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}