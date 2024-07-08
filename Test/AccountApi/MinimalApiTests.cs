using System.Net;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Test.Fixtures;
using Test.Helpers;

namespace Test.AccountApi;

[Collection("CustomWebApplicationFactoryTests")]
public class MinimalApiTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public MinimalApiTests(CustomWebApplicationFactory<Program> webApplicationFactory)
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

    [Theory]
    [InlineData("/customer")]
    [InlineData("/account")]
    public async Task Get_Endpoints_ReturnOKAndCorrectContentType(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_CustomerWithId_ReturnOKAndCorrectContentType()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await DataUtilities.ReinitializeDbForTestsAsync(context);
        var customer = await context.Customers.OrderBy(c => c.Id).FirstAsync();

        var response = await _client.GetAsync($"/customer/{customer.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_AccountWithId_ReturnOKAndCorrectContentType()
    {
        using var scope = _webApplicationFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await DataUtilities.ReinitializeDbForTestsAsync(context);
        var account = await context.Accounts.OrderBy(a => a.Id).FirstAsync();

        var response = await _client.GetAsync($"/account/{account.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}