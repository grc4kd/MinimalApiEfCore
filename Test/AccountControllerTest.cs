using System.Net;
using Api.Data;
using Api.Data.Seeding;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

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
        AccountDbSeeder seeder = new(db);
        seeder.SeedDatabase();

        MaxSeededAccountId = seeder.MaxSeededAccountId;
        MaxSeededCustomerId = seeder.MaxSeededCustomerId;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _factory.DisposeAsync();
    }

    [Fact]
    public async Task Post_OpenAccount_Created()
    {
        int customerId = 1;
        AccountType accountType = AccountType.Checking;
        var expectedAccountId = MaxSeededAccountId + 1;
        var request = new OpenAccountRequest(customerId, accountType, initialDeposit: 100);
        var expectedResponse = new OpenAccountResponse(customerId, expectedAccountId, succeeded: true);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/open", response.Headers.Location?.PathAndQuery);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<OpenAccountResponse>());
    }

    [Fact]
    public async Task Post_OpenAccountWithBadCustomerId_NotFound()
    {
        int invalidCustomerId = MaxSeededCustomerId + 1;

        var request = new OpenAccountRequest(invalidCustomerId, AccountType.Checking, initialDeposit: 100);
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<OpenAccountRequest>());
    }

    [Fact]
    public async void Open_CustomerAccount_VerifyAccountStatus()
    {
        var customerId = 1;
        var initialDeposit = 100;
        var expectedAccountId = MaxSeededAccountId + 1;
        var openAccountRequest = new OpenAccountRequest(customerId, AccountType.Savings, initialDeposit);
        
        var response = await _client.PostAsync("api/account/open", JsonContent.Create(openAccountRequest));
        
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        
        await using var scope = _factory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = await db.Accounts.FindAsync(expectedAccountId);

        Assert.NotNull(account);
        Assert.Equal(AccountStatus.OPEN, account.AccountStatus);
    }

    [Fact]
    public async Task Put_CloseAccount_OK()
    {
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
        int invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new CloseAccountRequest(invalidCustomerId, accountId: 1);
        
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccountWithBadAccountId_NotFound()
    {
        int invalidAccountId = MaxSeededAccountId + 1;
        var request = new CloseAccountRequest(customerId: 1, invalidAccountId);
        
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Put_CloseAccountWithUnrelatedAccountId_BadRequest()
    {
        var request = new CloseAccountRequest(customerId: 1, accountId: 2);
        
        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<CloseAccountRequest>());
    }

    [Fact]
    public async Task Post_Deposit_Created()
    {
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
        var invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new DepositRequest(invalidCustomerId, accountId: 1, 100);
        
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithBadAccountId_NotFound()
    {
        var invalidAccountId = MaxSeededAccountId + 1;
        var request = new DepositRequest(customerId: 1, invalidAccountId, amount: 100);
        
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_DepositWithUnrelatedAccountId_NotFound()
    {
        var request = new DepositRequest(customerId: 1, accountId: 2, 100);
        
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<DepositRequest>());
    }

    [Fact]
    public async Task Post_Withdrawal_Created()
    {
        var customerId = 2;
        var accountId = MaxSeededAccountId + 1;
        var initialAccountBalance = 100;
        var request = new WithdrawalRequest(customerId, accountId, amount: 100);
        var expectedResponse = new WithdrawalResponse(customerId, accountId, balance: 0, succeeded: true);
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await db.Accounts.AddAsync(new Account{
            Id = accountId,
            CustomerId = customerId,
            Balance = initialAccountBalance
        });
        await db.SaveChangesAsync();
        
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/withdrawal", response.Headers.Location?.PathAndQuery);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<WithdrawalResponse>());
    }

    [Fact]
    public async Task Post_WithdrawalWithBadCustomerId_NotFound()
    {
        var invalidCustomerId = MaxSeededCustomerId + 1;
        var request = new WithdrawalRequest(invalidCustomerId, accountId: 1, 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithBadAccountId_NotFound()
    {
        var invalidAccountId = MaxSeededAccountId + 1;
        var request = new WithdrawalRequest(customerId: 1, invalidAccountId, amount: 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_WithdrawalWithUnrelatedAccountId_NotFound()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 2, amount: 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equivalent(request, await response.Content.ReadFromJsonAsync<WithdrawalRequest>());
    }

    [Fact]
    public async Task Post_DepositWithNegativeAmount_BadRequest()
    {
        var request = new DepositRequest(customerId: 1, accountId: 1, amount: -100);
        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));
        var httpValidationProblemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.NotNull(httpValidationProblemDetails);
        Assert.Contains("Amount", httpValidationProblemDetails.Errors);
    }

    [Fact]
    public async Task Post_WithdrawalWithNegativeAmount_BadRequest()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: -100);
        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));
        var httpValidationProblemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.NotNull(httpValidationProblemDetails);
        Assert.Contains("Amount", httpValidationProblemDetails.Errors);
    }
}