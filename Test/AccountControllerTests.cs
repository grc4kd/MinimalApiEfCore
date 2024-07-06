using Domain.Accounts;
using Domain.Data;
using Api;
using Api.Requests;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Test;

public class AccountControllerTests
{
    [Fact]
    public async Task CloseAccount_PreconditionsMet_CloseAccountResponse()
    {
        int customerId = 1;
        int accountId = 1;
        var request = new CloseAccountRequest(customerId, accountId);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.CloseAsync(request))
            .ReturnsAsync(new CloseAccountResponse(customerId, accountId, true, AccountStatusType.OPEN));        
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ConvertActionResultToObjects<OkObjectResult, CloseAccountResponse>(
            await controller.Close(request));

        repositoryMock.Verify(r => r.CloseAsync(request));
        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.True(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
    }

    [Fact]
    public async Task Deposit_PreconditionsMet_DepositResponse()
    {
        int customerId = 1;
        int accountId = 1;
        decimal amount = 1;
        decimal expectedBalance = 101;
        Type expectedActionResultType = typeof(CreatedAtActionResult);
        Type expectedResponseType = typeof(DepositResponse);
        var request = new DepositRequest(customerId, accountId, amount);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.DepositAsync(request))
            .ReturnsAsync(new DepositResponse(customerId, accountId, expectedBalance, true));
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ConvertActionResultToObjects<CreatedAtActionResult, DepositResponse>(
            await controller.Deposit(request)
        );

        repositoryMock.Verify(r => r.DepositAsync(request));
        Assert.Equal(HttpStatusCode.Created, httpStatusCode);
        Assert.True(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(expectedBalance, response.Balance);
    }

    [Fact]
    public async Task Withdrawal_PreconditionsMet_WithdrawalResponse()
    {
        int customerId = 1;
        int accountId = 1;
        decimal amount = 1;
        decimal expectedBalance = 99;
        var request = new WithdrawalRequest(customerId, accountId, amount);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.WithdrawalAsync(request))
            .ReturnsAsync(new WithdrawalResponse(customerId, accountId, expectedBalance, true));
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ConvertActionResultToObjects<CreatedAtActionResult, WithdrawalResponse>(
            await controller.Withdrawal(request)
        );
        
        repositoryMock.Verify(r => r.WithdrawalAsync(request));
        Assert.Equal(HttpStatusCode.Created, httpStatusCode);
        Assert.True(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(expectedBalance, response.Balance);
    }

    [Fact]
    public async Task OpenAccount_PreconditionsMet_OpenAccountResponse()
    {
        int customerId = 1;
        int expectedAccountId = 1;
        AccountType accountType = AccountType.Savings;
        decimal amount = 100;
        var request = new OpenAccountRequest(customerId, accountType, amount);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.OpenAsync(request))
            .ReturnsAsync(new OpenAccountResponse(customerId, expectedAccountId, true));
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ConvertActionResultToObjects<CreatedAtActionResult, OpenAccountResponse>(
            await controller.Open(request)
        );

        repositoryMock.Verify(r => r.OpenAsync(request));
        Assert.Equal(HttpStatusCode.Created, httpStatusCode);
        Assert.True(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(expectedAccountId, response.AccountId);
    }

    private static (TResult, TResponse, HttpStatusCode) ConvertActionResultToObjects<TResult, TResponse>(IActionResult actionResult)
    {
        const string valuePropertyName = "Value";
        const string statusCodePropertyName = "StatusCode";

        TResult result = (TResult)actionResult;
        var getValueMethod = typeof(TResult).GetProperty(valuePropertyName)?.GetMethod;
        var resultValueObject = getValueMethod?.Invoke(result, null);
        
        if (resultValueObject == null)
        {
            Assert.Fail($"Couldn't get {valuePropertyName} of {typeof(TResult)} from {typeof(IActionResult)}.");
        }

        TResponse response = (TResponse)resultValueObject;
        var getStatusCodeMethod = typeof(TResult).GetProperty("StatusCode")?.GetMethod;
        var responseStatusCodeObject = getStatusCodeMethod?.Invoke(result, null);

        if (responseStatusCodeObject == null)
        {
            Assert.Fail($"Couldn't get {statusCodePropertyName} from {typeof(TResult)}");
        }

        HttpStatusCode httpStatusCode = (HttpStatusCode)responseStatusCodeObject;
        Assert.True(Enum.IsDefined(httpStatusCode));

        return (result, response, httpStatusCode);
    }
}