using Domain.Accounts;
using Domain.Data;
using Api;
using Api.Requests;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test;

public class AccountControllerTests
{
    private readonly Mock<IAccountRepository> repositoryMock;

    public AccountControllerTests()
    {
        repositoryMock = new Mock<IAccountRepository>();
    }

    [Fact]
    public async Task CloseAccount_PreconditionsMet_CloseAccountResponse()
    {
        var request = new CloseAccountRequest(1, 1);

        repositoryMock.Setup(r => r.CloseAsync(request))
            .ReturnsAsync(new CloseAccountResponse(1, 1, true, AccountStatusType.CLOSED));

        var controller = new AccountController(repositoryMock.Object);

        var response = await controller.Close(request);

        repositoryMock.Verify(r => r.CloseAsync(request));
        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async Task Deposit_PreconditionsMet_DepositResponse()
    {
        var request = new DepositRequest(1, 1, 1);

        repositoryMock.Setup(r => r.DepositAsync(request))
            .ReturnsAsync(new DepositResponse(1, 1, 101, true));

        var controller = new AccountController(repositoryMock.Object);

        var response = await controller.Deposit(request);

        repositoryMock.Verify(r => r.DepositAsync(request));
        Assert.IsType<CreatedAtActionResult>(response);
    }

    [Fact]
    public async Task Withdrawal_PreconditionsMet_WithdrawalResponse()
    {
        var request = new WithdrawalRequest(1, 1, 1);

        repositoryMock.Setup(r => r.WithdrawalAsync(request))
            .ReturnsAsync(new WithdrawalResponse(1, 1, 99, true));

        var controller = new AccountController(repositoryMock.Object);

        var response = await controller.Withdrawal(request);

        repositoryMock.Verify(r => r.WithdrawalAsync(request));
        Assert.IsType<CreatedAtActionResult>(response);
    }

    [Fact]
    public async Task OpenAccount_PreconditionsMet_OpenAccountResponse()
    {
        var request = new OpenAccountRequest(1, AccountType.Savings, 100);

        repositoryMock.Setup(r => r.OpenAsync(request))
            .ReturnsAsync(new OpenAccountResponse(1, 1, true));
        
        var controller = new AccountController(repositoryMock.Object);

        var response = await controller.Open(request);

        repositoryMock.Verify(r => r.OpenAsync(request));
        Assert.IsType<CreatedAtActionResult>(response);
    }
}