using System.Net;
using Api.Data;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Helpers;

namespace Test;

public class AccountControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountControllerTest(CustomWebApplicationFactory<Program> factory)
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
    public async Task Post_OpenAccount_Created()
    {
        await ResetDatabaseAsync();

        int customerId = 1;
        AccountType accountType = AccountType.Checking;

        var request = new OpenAccountRequest(customerId, accountType, initialDeposit: 100);
        var expectedResponse = new OpenAccountResponse(customerId, accountId: Utilities.MaxDbAccountId + 1, succeeded: true);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/open", response.Headers.Location?.PathAndQuery);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<OpenAccountResponse>());
    }

    [Fact]
    public async Task Post_OpenAccountWithBadCustomerId_NotFound()
    {
        await ResetDatabaseAsync();

        int invalidCustomerId = 0;

        var request = new OpenAccountRequest(invalidCustomerId, AccountType.Checking, initialDeposit: 100);
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<OpenAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccount_OK()
    {
        await ResetDatabaseAsync();

        var customerId = 1;
        var accountId = 1;
        var request = new CloseAccountRequest(customerId, accountId);
        var expectedResponse = new CloseAccountResponse(customerId, accountId, succeeded: true);
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<CloseAccountResponse>());
    }

    [Fact]
    public async Task Put_CloseAccountWithBadCustomerId_NotFound()
    {
        await ResetDatabaseAsync();

        int invalidCustomerId = Utilities.MaxDbCustomerId + 1;
        var request = new CloseAccountRequest(invalidCustomerId, accountId: 1);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccountWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        int invalidAccountId = Utilities.MaxDbAccountId + 1;
        var request = new CloseAccountRequest(customerId: 1, invalidAccountId);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccountWithUnrelatedAccountId_BadRequest()
    {
        await ResetDatabaseAsync();

        var request = new CloseAccountRequest(customerId: 1, accountId: 2);
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Post_Deposit_Created()
    {
        await ResetDatabaseAsync();

        int customerId = 1;
        int accountId = 1;
        var request = new DepositRequest(customerId, accountId, amount: 100);
        var expectedResponse = new DepositResponse(customerId, accountId, balance: 100, succeeded: true);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/deposit", response.Headers.Location?.PathAndQuery);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<DepositResponse>());
    }

    [Fact]
    public async void Post_DepositWithBadCustomerId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: Utilities.MaxDbCustomerId + 1, accountId: 1, 100);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: 1, accountId: Utilities.MaxDbAccountId + 1, 100);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithUnrelatedAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: 1, accountId: 2, 100);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithNegativeAmount_BadRequest()
    {
        await ResetDatabaseAsync();

        var request = new DepositRequest(customerId: 1, accountId: 1, -100);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_Withdrawal_Created()
    {
        await ResetDatabaseAsync();

        var customerId = 2;
        var accountId = 2;
        var request = new WithdrawalRequest(customerId, accountId, amount: 100);
        var expectedResponse = new WithdrawalResponse(customerId, accountId, balance: 100, succeeded: true);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/withdrawal", response.Headers.Location?.PathAndQuery);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<WithdrawalResponse>());
    }

    [Fact]
    public async Task Post_WithdrawalWithBadCustomerId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: Utilities.MaxDbCustomerId + 1, accountId: 1, 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: Utilities.MaxDbAccountId + 1, 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithUnrelatedAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 2, 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithNegativeValue_BadRequest()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 1, -100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }
}