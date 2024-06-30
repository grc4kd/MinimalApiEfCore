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

        var request = new HttpRequestMessage(HttpMethod.Post, "/account/open")
        {
            Content = JsonContent.Create(new OpenAccount(customerId, accountType, initialDeposit))
        };
        var expected = new OpenAccountResponse 
        {
            CustomerId = customerId,
            AccountId = expectedAccountId,
            Succeeded = expectedSucceeded
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/account", response.Headers.Location?.OriginalString);
        Assert.Equivalent(expected, await response.Content.ReadFromJsonAsync<OpenAccountResponse>());
    }
}