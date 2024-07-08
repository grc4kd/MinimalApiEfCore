using Api.Requests;
using Domain.Accounts.Data;

namespace Test;

public class DataModelTests
{
    [Fact]
    public void Account_Close_ShouldSetAccountStatus()
    {
        var account = new Account
        {
            AccountStatus = new AccountStatus { AccountStatusType = AccountStatusType.OPEN }
        };

        account.Close();

        Assert.Equal(AccountStatusType.CLOSED, account.AccountStatus.AccountStatusType);
    }

    [Fact]
    public void Account_MakeDeposit_ShouldAddToBalance_Decimal()
    {
        var account = new Account
        {
            Balance = 100
        };

        account.MakeDeposit(100m);

        Assert.Equal(200, account.Balance);
    }

    [Fact]
    public void Account_MakeDepositWithNegativeValue_ArgumentOutOfRangeException()
    {
        var account = new Account();

        Assert.Throws<ArgumentOutOfRangeException>(() => account.MakeDeposit(-100m));
    }

    [Fact]
    public void DepositRequest_UsesColumnTypeCode_Decimal()
    {
        var request = new DepositRequest(customerId: 1, accountId: 1, amount: 100);

        Assert.Equal(TypeCode.Decimal, request.Amount.GetTypeCode());
    }

    [Fact]
    public void WithdrawalRequest_UsesColumnTypeCode_Decimal()
    {
        var request = new WithdrawalRequest(customerId: 1, accountId: 1, amount: 100);

        Assert.Equal(TypeCode.Decimal, request.Amount.GetTypeCode());
    }

    [Fact]
    public void Account_WithdrawalWithInsufficientFunds_ThrowsException()
    {
        var account = new Account
        {
            Balance = 100
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => account.MakeWithdrawal(amount: 100.01m));
    }

    [Fact]
    public void Customer_HasSavingsAccount_TrueTest()
    {
        var customer = new Customer() { Name = string.Empty };
        customer.Accounts.Add(new Account { AccountType = AccountType.Savings });

        Assert.True(customer.HasSavingsAccount());
    }

    [Fact]
    public void Customer_HasSavingsAccount_FalseTest()
    {
        var customer = new Customer() { Name = string.Empty };

        Assert.False(customer.HasSavingsAccount());
    }

    [Fact]
    public void Customer_OpenAccount_ReturnsNewAccountForCustomer()
    {
        var initialDeposit = 100m;
        var accountType = AccountType.Savings;
        var customer = new Customer() { Id = 1, Name = string.Empty };

        var account = customer.OpenAccount(accountType, initialDeposit);

        Assert.Contains(account, customer.Accounts);
        Assert.Equal(initialDeposit, account.Balance);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(AccountStatusType.OPEN, account.AccountStatus.AccountStatusType);
        Assert.Equal(customer.Id, account.CustomerId);
    }
}