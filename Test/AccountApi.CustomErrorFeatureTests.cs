using System.Net;
using Infrastructure;
using Api.Requests;
using Api.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Test.Fixtures;
using Domain.Accounts.Data;

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

    
}