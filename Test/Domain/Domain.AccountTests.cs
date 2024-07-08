using Domain.Accounts.Data;
using Moq;

namespace Test.Domain;

public class AccountTests
{
    private readonly Mock<Customer> CustomerMock = new();

    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public void Account_CloseAtZeroBalance_VerifyData(AccountType accountType)
    {
        var accountStatus = new AccountStatus { AccountStatusType = AccountStatusType.OPEN };
        decimal balance = 0;
        int customerId = 1;
        int id = 1;
        var account = new Account
        {
            AccountStatus = accountStatus,
            AccountType = accountType,
            Balance = balance,
            Customer = CustomerMock.Object,
            CustomerId = customerId,
            Id = id
        };

        account.Close();

        CustomerMock.VerifyNoOtherCalls();
        Assert.Equal(AccountStatusType.CLOSED, account.AccountStatus.AccountStatusType);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(balance, account.Balance);
        Assert.Equal(customerId, account.CustomerId);
        Assert.Equal(id, account.Id);
    }

    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public void Account_MakeDeposit_VerifyData(AccountType accountType)
    {

        var accountStatus = new AccountStatus { AccountStatusType = AccountStatusType.OPEN };
        decimal balance = 100;
        decimal amount = 20;
        int customerId = 1;
        int id = 1;
        var account = new Account
        {
            AccountStatus = accountStatus,
            AccountType = accountType,
            Balance = balance,
            Customer = CustomerMock.Object,
            CustomerId = customerId,
            Id = id
        };

        account.MakeDeposit(amount);

        CustomerMock.VerifyNoOtherCalls();
        Assert.Equal(AccountStatusType.OPEN, account.AccountStatus.AccountStatusType);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(balance + amount, account.Balance);
        Assert.Equal(customerId, account.CustomerId);
        Assert.Equal(id, account.Id);
    }

    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public void Account_MakeWithdrawal_VerifyData(AccountType accountType)
    {

        var accountStatus = new AccountStatus { AccountStatusType = AccountStatusType.OPEN };
        decimal balance = 100;
        decimal amount = 20;
        int customerId = 1;
        int id = 1;
        var account = new Account
        {
            AccountStatus = accountStatus,
            AccountType = accountType,
            Balance = balance,
            Customer = CustomerMock.Object,
            CustomerId = customerId,
            Id = id
        };

        account.MakeWithdrawal(amount);

        CustomerMock.VerifyNoOtherCalls();
        Assert.Equal(AccountStatusType.OPEN, account.AccountStatus.AccountStatusType);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(balance - amount, account.Balance);
        Assert.Equal(customerId, account.CustomerId);
        Assert.Equal(id, account.Id);
    }
}