using System.Net;
using Domain.Data;
using Infrastructure;
using Api.Requests;
using Api.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Fixtures;

namespace Test;

[Collection("CustomWebApplicationFactoryTests")]
public class AccountApiTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    private CustomWebApplicationFactory<Program> _webApplicationFactory;

    public AccountApiTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;

        _client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _webApplicationFactory.CleanupAsync();
    }

    private async Task<Account> GetTestAccount()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        return await context.Accounts.OrderBy(a => a.Id).LastAsync();
    }

    [Fact]
    public async Task Post_OpenAccount_Created()
    {
        var account = await GetTestAccount();

        var request = new OpenAccountRequest(account.CustomerId, AccountType.Savings, initialDeposit: 100);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/open", response.Headers.Location?.LocalPath);
        Assert.StartsWith($"?id=", response.Headers.Location?.Query);

        var openAccountResponse = await response.Content.ReadFromJsonAsync<OpenAccountResponse>();
        Assert.NotNull(openAccountResponse);
        Assert.Equal(account.CustomerId, openAccountResponse.CustomerId);
        Assert.True(openAccountResponse.AccountId > 0);
        Assert.True(openAccountResponse.Succeeded);

        using var assertScope = _webApplicationFactory.Services.CreateAsyncScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var assertAccount = await assertContext.Accounts.FindAsync(openAccountResponse.AccountId);

        Assert.NotNull(assertAccount);
        Assert.Equal(AccountStatusType.OPEN, assertAccount.AccountStatus.AccountStatusType);
        Assert.Equal(100, assertAccount.Balance);
        Assert.Equal(AccountType.Savings, assertAccount.AccountType);
        Assert.Equal(account.CustomerId, assertAccount.CustomerId);
    }

    [Fact]
    public async Task Post_Deposit_Created()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = new Account
        {
            AccountStatus = new AccountStatus(AccountStatusType.OPEN),
            Balance = 100,
            Customer = new Customer { Name = string.Empty }
        };
        await context.AddAsync(account);
        await context.SaveChangesAsync();

        var request = new DepositRequest(account.CustomerId, account.Id, amount: 100);
        var expectedResponse = new DepositResponse(account.CustomerId, account.Id, balance: 200, succeeded: true);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/deposit", response.Headers.Location?.LocalPath);
        Assert.Equal($"?id={account.Id}", response.Headers.Location?.Query);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<DepositResponse>());

        using var assertScope = _webApplicationFactory.Services.CreateAsyncScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var assertAccount = await assertContext.Accounts.FindAsync(account.Id);

        Assert.NotNull(assertAccount);
        Assert.Equal(200, assertAccount.Balance);
    }

    [Fact]
    public async Task Post_Withdrawal_Created()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = new Account
        {
            AccountStatus = new AccountStatus(AccountStatusType.OPEN),
            Balance = 100,
            Customer = new Customer { Name = string.Empty }
        };
        await context.AddAsync(account);
        await context.SaveChangesAsync();

        var request = new WithdrawalRequest(account.CustomerId, account.Id, amount: 100);
        var expectedResponse = new WithdrawalResponse(account.CustomerId, account.Id, balance: 0, succeeded: true);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Account/withdrawal", response.Headers.Location?.LocalPath);
        Assert.Equal($"?id={account.Id}", response.Headers.Location?.Query);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<WithdrawalResponse>());

        using var assertScope = _webApplicationFactory.Services.CreateAsyncScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var assertAccount = await assertContext.Accounts.FindAsync(account.Id);

        Assert.NotNull(assertAccount);
        Assert.Equal(0, assertAccount.Balance);
    }

    [Fact]
    public async Task Put_CloseAccountWithZeroBalance_OK()
    {
        var account = await GetTestAccount();

        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        context.Attach(account);
        account.Balance = 0;
        await context.SaveChangesAsync();

        var request = new CloseAccountRequest(account.CustomerId, account.Id);
        var expectedResponse = new CloseAccountResponse(account.CustomerId, account.Id, succeeded: true, 
            AccountStatusType.CLOSED);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedResponse, await response.Content.ReadFromJsonAsync<CloseAccountResponse>());

        using var assertScope = _webApplicationFactory.Services.CreateAsyncScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var assertAccount = await assertContext.Accounts.FindAsync(account.Id);

        Assert.NotNull(assertAccount);
        Assert.Equal(AccountStatusType.CLOSED, assertAccount.AccountStatus.AccountStatusType);
    }

    [Fact]
    public async Task Put_CloseAccountWithGreaterThanZeroBalance_BadRequest()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = new Account
        {
            AccountStatus = new AccountStatus(AccountStatusType.OPEN),
            Balance = 100,
            Customer = new Customer { Name = string.Empty }
        };
        await context.AddAsync(account);
        await context.SaveChangesAsync();

        var request = new CloseAccountRequest(account.CustomerId, account.Id);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_OpenAccountWithBadCustomerId_NotFound()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        int invalidCustomerId = await context.Customers.MaxAsync(c => c.Id) + 1;

        var request = new OpenAccountRequest(invalidCustomerId, AccountType.Checking, initialDeposit: 100);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var customerNotFoundResponse = await response.Content.ReadFromJsonAsync<CustomerNotFoundResponse>();
        Assert.NotNull(customerNotFoundResponse);
        Assert.False(customerNotFoundResponse.Succeeded);
        Assert.Equal(request.CustomerId, customerNotFoundResponse.CustomerId);
    }

    [Fact]
    public async Task Post_OpenCheckingAccountBeforeSavingsAccount_BadRequest()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var customer = new Customer { Name = string.Empty };
        await context.AddAsync(customer);
        await context.SaveChangesAsync();

        var request = new OpenAccountRequest(customer.Id, AccountType.Checking, 100m);

        var response = await _client.PostAsync("api/account/open", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var savingsAccountDoesNotExistResponse = await response.Content.ReadFromJsonAsync<SavingsAccountDoesNotExistResponse>();
        Assert.NotNull(savingsAccountDoesNotExistResponse);
        Assert.False(savingsAccountDoesNotExistResponse.Succeeded);
        Assert.Equal(request.CustomerId, savingsAccountDoesNotExistResponse.CustomerId);
    }

    [Fact]
    public async Task Post_DepositWithUnrelatedAccountId_NotFound()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        using var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var customer = await context.Customers.OrderBy(c => c.Id).FirstAsync();
        var unrelatedAccount = await context.Accounts.OrderBy(a => a.Id).FirstAsync(a => a.CustomerId != customer.Id);

        var request = new DepositRequest(customer.Id, unrelatedAccount.Id, 100);

        var response = await _client.PostAsync("api/account/deposit", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var accountNotFoundResponse = await response.Content.ReadFromJsonAsync<AccountNotFoundResponse>();
        Assert.NotNull(accountNotFoundResponse);
        Assert.False(accountNotFoundResponse.Succeeded);
        Assert.Equal(request.AccountId, accountNotFoundResponse.AccountId);
    }

    [Fact]
    public async Task Post_WithdrawalWithUnrelatedAccountId_NotFound()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        using var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var customer = await context.Customers.OrderBy(c => c.Id).FirstAsync();
        var unrelatedAccount = await context.Accounts.OrderBy(a => a.Id).FirstAsync(a => a.CustomerId != customer.Id);

        var request = new WithdrawalRequest(customer.Id, unrelatedAccount.Id, amount: 100);

        var response = await _client.PostAsync("api/account/withdrawal", JsonContent.Create(request));
        var accountNotFoundResponse = await response.Content.ReadFromJsonAsync<AccountNotFoundResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(accountNotFoundResponse);
        Assert.False(accountNotFoundResponse.Succeeded);
        Assert.Equal(request.AccountId, accountNotFoundResponse.AccountId);
    }

    [Fact]
    public async Task Put_CloseAccountWithUnrelatedAccountId_NotFound()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        using var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var customer = await context.Customers.OrderBy(c => c.Id).FirstAsync();
        var unrelatedAccount = await context.Accounts.OrderBy(a => a.Id).FirstAsync(a => a.CustomerId != customer.Id);

        var request = new CloseAccountRequest(customer.Id, unrelatedAccount.Id);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));
        var accountNotFoundResponse = await response.Content.ReadFromJsonAsync<AccountNotFoundResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(accountNotFoundResponse);
        Assert.False(accountNotFoundResponse.Succeeded);
        Assert.Equal(request.AccountId, accountNotFoundResponse.AccountId);
    }

    [Fact]
    public async void Put_CloseAccountAlreadyClosed_BadRequest()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        using var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        var account = new Account
        {
            AccountStatus = new AccountStatus(AccountStatusType.CLOSED),
            Customer = new Customer { Name = string.Empty }
        };
        await context.AddAsync(account);
        await context.SaveChangesAsync();

        var request = new CloseAccountRequest(account.CustomerId, account.Id);

        var response = await _client.PutAsync("api/account/close", JsonContent.Create(request));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}