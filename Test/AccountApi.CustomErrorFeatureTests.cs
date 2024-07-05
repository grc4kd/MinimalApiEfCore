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
public class AccountApiCustomErrorFeatureTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    private CustomWebApplicationFactory<Program> _webApplicationFactory;

    public AccountApiCustomErrorFeatureTests(CustomWebApplicationFactory<Program> webApplicationFactory)
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
    public async Task Post_WithdrawalWithInsufficientFundBalance_BadRequest()
    {
        var account = await GetTestAccount();

        var request = new WithdrawalRequest(account.CustomerId, account.Id, amount: 100.01m);

        var response = _client.PostAsync("api/account/withdrawal", JsonContent.Create(request)).Result;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var accountNotFoundResponse = await response.Content.ReadFromJsonAsync<AccountNotFoundResponse>();

        Assert.NotNull(accountNotFoundResponse);
        Assert.False(accountNotFoundResponse.Succeeded);
        Assert.Equal(request.AccountId, accountNotFoundResponse.AccountId);
    }
}