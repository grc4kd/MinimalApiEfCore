using System.Net;
using Domain.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Fixtures;
using Api.Requests;

namespace Test;

[Collection("CustomWebApplicationFactoryTests")]
public class AccountApiActionFilterTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public AccountApiActionFilterTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _client = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _webApplicationFactory.CleanupAsync();
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

    [Fact]
    public async void Post_DepositWithNegativeAmount_BadRequest()
    {
        var request = new DepositRequest(customerId: 1, accountId: 1, amount: -100);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var modelStateDictionary = await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string[]>>();

        Assert.NotNull(modelStateDictionary);
        Assert.Contains(modelStateDictionary, f => f.Key == nameof(DepositRequest.Amount));
    }

    [Fact]
    public async Task Post_WithdrawalWithNegativeAmount_BadRequest()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: -100);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var modelStateDictionary = await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string[]>>();

        Assert.NotNull(modelStateDictionary);
        Assert.Contains(modelStateDictionary, f => f.Key == nameof(WithdrawalRequest.Amount));
    }
}