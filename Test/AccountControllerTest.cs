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

        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AccountDbContext>();

        Utilities.InitializeDbForTests(db);
    }

    private void ResetDatabase() {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AccountDbContext>();

        Utilities.ReinitializeDbForTests(db);
    }

    [Fact]
    public async Task PostOpenAccountRequest_ParseResponse()
    {
        ResetDatabase();
        
        int customerId = 1;
        AccountType accountType = AccountType.Checking;
        double initialDeposit = 100;

        var request = new HttpRequestMessage(HttpMethod.Post, "api/account/open")
        {
            Content = JsonContent.Create(new OpenAccountRequest(customerId, accountType, initialDeposit))
        };

        var response = await _client.SendAsync(request);
        var openAccountResponse = await response.Content.ReadFromJsonAsync<OpenAccountResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/Account/open", response.Headers.Location?.PathAndQuery);
        Assert.NotNull(openAccountResponse);
        Assert.True(openAccountResponse.Succeeded);
        Assert.Equal(customerId, openAccountResponse.CustomerId);
        Assert.True(openAccountResponse.AccountId > 0);
    }

    [Fact]
    public async Task PostOpenAccountRequest_BadCustomerId_ParseErrorResponse()
    {
        ResetDatabase();
        
        int invalidCustomerId = Utilities.MaxDbCustomerId + 1;
        AccountType accountType = AccountType.Checking;
        double initialDeposit = 100;

        var request = new HttpRequestMessage(HttpMethod.Post, "api/account/open")
        {
            Content = JsonContent.Create(new OpenAccountRequest(invalidCustomerId, accountType, initialDeposit))
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}