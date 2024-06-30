using System.Net;
using Api.Data;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test;

public class ApiTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ApiTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/customer")]
    [InlineData("/account")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Post_OpenAccount_ReturnsCreated()
    {
        int customerId = 1;
        AccountType accountType = AccountType.Checking;
        double initialDeposit = 100;
        int expectedAccountId = 1;
        bool expectedSucceeded = true;
        var expected = new OpenAccountResponse(customerId, expectedAccountId, expectedSucceeded);

        var request = new HttpRequestMessage(HttpMethod.Post, "/account/open")
        {
            Content = JsonContent.Create(new OpenAccount(customerId, accountType, initialDeposit))
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/account/{customerId}", response.Headers.Location?.OriginalString);
        Assert.Equivalent(expected, await response.Content.ReadFromJsonAsync<OpenAccountResponse>());
    }

    [Fact]
    public async Task Put_CloseAccount_ReturnsOkWithContent()
    {
        int customerId = 1;
        int accountId = 1;
        bool expectedSucceeded = true;
        var expected = new CloseAccountResponse(customerId, accountId, expectedSucceeded);

        var request = new HttpRequestMessage(HttpMethod.Put, "/account/close")
        {
            Content = JsonContent.Create(new CloseAccount(customerId, accountId))
        };

        var response = await _client.SendAsync(request);
        var closeAccountResponse = await response.Content.ReadFromJsonAsync<CloseAccountResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expected, closeAccountResponse);
    }

    [Fact]
    public async void Post_Deposit_ReturnsCreatedWithContent()
    {
        int customerId = 1;
        int accountId = 1;
        double amount = 100;
        bool expectedSucceeded = true;
        var expected = new DepositResponse(customerId, accountId, amount, expectedSucceeded);

        var request = new HttpRequestMessage(HttpMethod.Post, "/account/deposit")
        {
            Content = JsonContent.Create(new Deposit(customerId, accountId, amount))
        };

        var response = await _client.SendAsync(request);
        var depositResponse = await response.Content.ReadFromJsonAsync<DepositResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equivalent(expected, depositResponse);
    }

    [Fact]
    public async void Post_Withdrawal_ReturnsCreatedWithContent()
    {
        int customerId = 1;
        int accountId = 1;
        double amount = 50;
        bool expectedSucceeded = true;
        var expected = new WithdrawalResponse(customerId, accountId, amount, expectedSucceeded);

        var request = new HttpRequestMessage(HttpMethod.Post, "/account/withdrawal")
        {
            Content = JsonContent.Create(new Withdrawal(customerId, accountId, amount))
        };

        var response = await _client.SendAsync(request);
        var withdrawalResponse = await response.Content.ReadFromJsonAsync<WithdrawalResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equivalent(expected, withdrawalResponse);
    }
}