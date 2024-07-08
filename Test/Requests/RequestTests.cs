using Api.Requests;
using Domain.Accounts.Data;

namespace Test.Requests;

public class RequestTests
{
    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public void OpenAccount_TestRequestDefinition(AccountType accountType)
    {
        int customerId = 5;
        decimal initialDeposit = 100;

        var request = new OpenAccountRequest(customerId, accountType, initialDeposit);

        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(accountType, request.AccountType);
        Assert.Equal(initialDeposit, request.InitialDeposit);
    }

    [Fact]
    public void CloseAccount_TestRequestDefinition()
    {
        int customerId = 5;
        int accountId = 17;

        var request = new CloseAccountRequest(customerId, accountId);

        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(accountId, request.AccountId);
    }

    [Fact]
    public void Deposit_TestRequestDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        decimal amount = 20;

        var request = new DepositRequest(customerId, accountId, amount);

        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(accountId, request.AccountId);
        Assert.Equal(amount, request.Amount);
    }

    [Fact]
    public void Withdrawal_TestRequestDefinition()
    {
        int customerId = 5;
        int accountId = 17;
        decimal amount = 10;

        var request = new WithdrawalRequest(customerId, accountId, amount);

        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(accountId, request.AccountId);
        Assert.Equal(amount, request.Amount);
    }
}
