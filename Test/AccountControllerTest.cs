using System.Net;
using Api.Data;
using Api.Data.Seeding;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;

namespace Test;

public class AccountControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    private static int MaxSeededAccountId;
    private static int MaxSeededCustomerId;

    public AccountControllerTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        db.Database.Migrate();
    }

    private async Task ResetDatabaseAsync() {
        await using var scope = _factory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        db.RemoveRange(db.Customers);
        db.RemoveRange(db.Accounts);
        await db.SaveChangesAsync();
        await AccountDbSeeder.SeedDatabaseAsync(db);

        MaxSeededAccountId = AccountDbSeeder.MaxSeededAccountId;
        MaxSeededCustomerId = AccountDbSeeder.MaxSeededCustomerId;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _factory.DisposeAsync();
    }

    [Fact]
    public async Task Post_OpenAccount_Created()
    {
        await ResetDatabaseAsync();

        int customerId = 1;
        AccountType accountType = AccountType.Savings;
        var request = new OpenAccountRequest(customerId, accountType, initialDeposit: 100);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/open", response.Headers.Location?.PathAndQuery);

        var openAccountResponse = await response.Content.ReadFromJsonAsync<OpenAccountResponse>();
        Assert.NotNull(openAccountResponse);
        Assert.Equal(customerId, openAccountResponse.CustomerId);
        Assert.True(openAccountResponse.AccountId > 0);
        Assert.True(openAccountResponse.Succeeded);
    }

    [Fact]
    public async Task Post_OpenAccountWithBadCustomerId_NotFound()
    {
        await ResetDatabaseAsync();

        int invalidCustomerId = MaxSeededCustomerId + 1;

        var request = new OpenAccountRequest(invalidCustomerId, AccountType.Checking, initialDeposit: 100);
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<OpenAccountRequest>());
    }

    [Fact]
    public async void Open_CustomerAccount_VerifyAccountValues()
    {
        await ResetDatabaseAsync();

        var customerId = 1;
        var initialDeposit = 100;
        var openAccountRequest = new OpenAccountRequest(customerId, AccountType.Savings, initialDeposit);
        
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(openAccountRequest));
        
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        
        var openAccountResponse = await response.Content.ReadFromJsonAsync<OpenAccountResponse>();

        Assert.NotNull(openAccountResponse);
        Assert.Equal(customerId, openAccountResponse.CustomerId);
        Assert.True(openAccountResponse.AccountId > 0);
        Assert.True(openAccountResponse.Succeeded);

        await using var scope = _factory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = await db.Accounts.FindAsync(openAccountResponse.AccountId);

        Assert.NotNull(account);
        Assert.Equal(AccountStatus.OPEN, account.AccountStatus);
        Assert.Equal(initialDeposit, account.Balance);
        Assert.Equal(customerId, account.CustomerId);
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

        int invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new CloseAccountRequest(invalidCustomerId, accountId: 1);
        
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccountWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        int invalidAccountId = MaxSeededAccountId + 1;
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

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Post_Deposit_Created()
    {
        await ResetDatabaseAsync();

        int customerId = 1;
        int accountId = 1;
        var request = new DepositRequest(customerId, accountId, amount: 100);
        var expectedResponse = new DepositResponse(customerId, accountId, balance: 200, succeeded: true);
     
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

        var invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new DepositRequest(invalidCustomerId, accountId: 1, 100);
        
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var invalidAccountId = MaxSeededAccountId + 1;
        var request = new DepositRequest(customerId: 1, invalidAccountId, amount: 100);
        
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
    public async Task Post_Withdrawal_Created()
    {
        await ResetDatabaseAsync();

        var customerId = 1;
        var accountId = 1;
        var request = new WithdrawalRequest(customerId, accountId, amount: 100);
        var expectedResponse = new WithdrawalResponse(customerId, accountId, balance: 0, succeeded: true);

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

        var invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new WithdrawalRequest(invalidCustomerId, accountId: 1, 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithBadAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var invalidAccountId = MaxSeededAccountId + 1;
        var request = new WithdrawalRequest(customerId: 1, invalidAccountId, amount: 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithUnrelatedAccountId_NotFound()
    {
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 2, amount: 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_DepositWithNegativeAmount_BadRequest()
    {
        await ResetDatabaseAsync();

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
        await ResetDatabaseAsync();

        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: -100);
        
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var modelStateDictionary = await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string[]>>();

        Assert.NotNull(modelStateDictionary);
        Assert.Contains(modelStateDictionary, f => f.Key == nameof(WithdrawalRequest.Amount));
    }

    [Fact]
    public async Task Post_WithdrawalWithInsufficientFundBalance_BadRequest()
    {
        await ResetDatabaseAsync();

        var customerId = 1;
        var accountId = 1;
        var request = new WithdrawalRequest(customerId, accountId, amount: 100.01m);
        
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Insufficient Funds", problemDetails.Title);
    }

    [Fact]
    public async Task Post_OpenCheckingAccountBeforeSavingsAccount_BadRequest()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var customer = new Customer {
            Name = "Frank"
        };
        await db.AddAsync(customer);
        await db.SaveChangesAsync();

        var request = new OpenAccountRequest(customer.Id, AccountType.Checking, 100m);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var modelStateDictionary = await response.Content.ReadFromJsonAsync<IReadOnlyDictionary<string, string[]>>();

        Assert.NotNull(modelStateDictionary);
        Assert.Contains(nameof(AccountType), modelStateDictionary);
    }
}