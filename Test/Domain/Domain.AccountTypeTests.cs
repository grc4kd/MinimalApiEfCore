using Domain.Accounts.Data;

namespace Test.Domain;

public class AccountTypeTests
{
    [Theory]
    [InlineData(1, AccountType.Checking)]
    [InlineData(2, AccountType.Savings)]
    public void AccountStatusType_EnumMembers_VerifyConstantValues(int constValue, AccountType enumMember)
    {
        Assert.Equal(constValue, (int)enumMember);
    }
}