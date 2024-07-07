using Api.Responses;
using Domain.Accounts.Data;

namespace Test;

public class ResponseTests
{
    [Fact]
    public void OpenAccountResponse_TestResponseDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        bool succeeded = true;

        var response = new OpenAccountResponse(customerId, accountId, succeeded);

        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(succeeded, response.Succeeded);
    }

    [Fact]
    public void CloseAccountResponse_TestResponseDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        bool succeeded = true;

        var response = new CloseAccountResponse(customerId, accountId, succeeded, AccountStatusType.CLOSED);

        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(succeeded, response.Succeeded);
        Assert.Equal(AccountStatusType.CLOSED, response.AccountStatus);
    }

    [Fact]
    public void DepositResponse_TestResponseDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        decimal balance = 120;
        bool succeeded = true;

        var response = new DepositResponse(customerId, accountId, balance, succeeded);

        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(balance, response.Balance);
        Assert.Equal(succeeded, response.Succeeded);
    }

    [Fact]
    public void WithdrawalResponse_TestResponseDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        decimal balance = 80;
        bool succeeded = true;

        var response = new WithdrawalResponse(customerId, accountId, balance, succeeded);

        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(balance, response.Balance);
        Assert.Equal(succeeded, response.Succeeded);
    }
}
