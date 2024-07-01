using Api.Data;
using Api.Request;

namespace Test;

public class DataModelTest
{
    [Fact]
    public void Account_Close_ShouldSetAccountStatus()
    {
        var account = new Account {
            AccountStatus = AccountStatus.OPEN
        };

        account.Close();

        Assert.Equal(AccountStatus.CLOSED, account.AccountStatus);
    }

    [Fact]
    public void Account_MakeDeposit_ShouldAddToBalance_Decimal()
    {
        var account = new Account {
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
    public void Account_MakeWithdrawalWithNegativeValue_ArgumentOutOfRangeException()
    {
        var account = new Account();

        Assert.Throws<ArgumentOutOfRangeException>(() => account.MakeWithdrawal(-100m));
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
}