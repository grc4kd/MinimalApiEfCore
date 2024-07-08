using Domain.Accounts.Data;
using Moq;

namespace Test.Domain;

public class CustomerTests
{
    [Fact]
    public void Customer_SavingsAccountExists_HasSavingsAccountShouldReturnTrue()
    {
        var account = new Account 
        {
            AccountStatus = new AccountStatus { AccountStatusType = AccountStatusType.OPEN },
            AccountType = AccountType.Savings,
            Balance = 1000,
            Customer = null!,
            CustomerId = 2,
            Id = 2
        };
        
        var customer = new Customer { Id = 2, Name = "William Riker", Accounts = [ account ] };
        account.Customer = customer;

        Assert.True(customer.HasSavingsAccount());
    }

    [Fact]
    public void Customer_SavingsAccountDoesNotExist_HasSavingsAccountShouldReturnFalse()
    {        
        var customer = new Customer { Id = 2, Name = "William Riker" };

        Assert.False(customer.HasSavingsAccount());
    }

    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public void Customer_OpenAccount_ReturnsNewAccount(AccountType accountType)
    {
        int customerId = 3;
        decimal initialDeposit = 100;
        var customer = new Customer { Id = customerId, Name = "Geordi La Forge" };
        
        var account = customer.OpenAccount(accountType, initialDeposit: initialDeposit);

        Assert.Equal(AccountStatusType.OPEN, account.AccountStatus.AccountStatusType);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(initialDeposit, account.Balance);
        Assert.Null(account.Customer);
        // these properties are initialized by the ORM
        Assert.Equal(customerId, account.CustomerId);
        Assert.Equal(0, account.Id);
    }

    [Fact]
    public void Customer_ToString_TestFormat()
    {
        var customerName = "Carl";

        var customer = new Customer { 
            Id = 1, 
            Name = customerName
        };

        Assert.Equal("Customer# 00000001; Name = Carl", customer.ToString());
    }
}