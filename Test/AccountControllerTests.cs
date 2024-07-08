using Domain.Accounts;
using Api;
using Api.Requests;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Domain.Accounts.Data;
using Test.Helpers;
using FluentValidation.Results;

namespace Test;

public class AccountControllerTests
{
    #region Critical Path

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

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<OkObjectResult, CloseAccountResponse>(
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

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<CreatedAtActionResult, DepositResponse>(
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

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<CreatedAtActionResult, WithdrawalResponse>(
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

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<CreatedAtActionResult, OpenAccountResponse>(
            await controller.Open(request)
        );

        repositoryMock.Verify(r => r.OpenAsync(request));
        Assert.Equal(HttpStatusCode.Created, httpStatusCode);
        Assert.True(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(expectedAccountId, response.AccountId);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task OpenAccount_NoSavingsAccount_ShouldReturnSavingsAccountDoesNotExistResponse()
    {
        int customerId = 1;
        AccountType accountType = AccountType.Checking;
        decimal amount = 100;
        var request = new OpenAccountRequest(customerId, accountType, amount);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.OpenAsync(request))
            .ReturnsAsync(new SavingsAccountDoesNotExistResponse(customerId));
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<BadRequestObjectResult, SavingsAccountDoesNotExistResponse>(
            await controller.Open(request)
        );

        repositoryMock.Verify(r => r.OpenAsync(request));
        Assert.Equal(HttpStatusCode.BadRequest, httpStatusCode);
        Assert.False(response.Succeeded);
        Assert.Equal(customerId, response.CustomerId);
    }

    [Fact]
    public async Task OpenAccount_DepositLessThanMinimum_ShouldReturn___Async()
    {
        int customerId = 1;
        AccountType accountType = AccountType.Checking;
        decimal amount = 10;
        var request = new OpenAccountRequest(customerId, accountType, amount);
        var repositoryMock = new Mock<IAccountRepository>();
        repositoryMock.Setup(r => r.OpenAsync(request))
            .ReturnsAsync(new OpenAccountValidationResponse(customerId, new ValidationResult(
                [ new ValidationFailure("Amount", "placeholder text") ]
            )));
        var controller = new AccountController(repositoryMock.Object);

        var (result, response, httpStatusCode) = ActionResultUtilities.ConvertActionResultToObjects<BadRequestObjectResult, OpenAccountValidationResponse>(
            await controller.Open(request)
        );
        
        repositoryMock.Verify(r => r.OpenAsync(request));
        Assert.False(response.Succeeded);
        Assert.Single(response.ValidationResult.Errors);
        Assert.Contains(response.ValidationResult.Errors, e => e.PropertyName == "Amount");
    }

    #endregion
}