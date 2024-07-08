using Domain.Accounts.Data;

namespace Test.Domain;

public class AccountStatusTests
{
    [Theory]
    [InlineData(AccountStatusType.CLOSED)]
    [InlineData(AccountStatusType.OPEN)]
    public void AccountStatus_AccountStatusTypes_VerifyData(AccountStatusType accountStatusType)
    {
        var accountStatus = new AccountStatus { AccountStatusType = accountStatusType };

        Assert.Equal(accountStatusType, accountStatus.AccountStatusType);
    }
}