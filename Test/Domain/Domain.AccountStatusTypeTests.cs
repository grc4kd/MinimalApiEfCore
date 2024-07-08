using Domain.Accounts.Data;

namespace Test.Domain;

public class AccountStatusTypeTests
{
    [Theory]
    [InlineData(0, AccountStatusType.OPEN)]
    [InlineData(1, AccountStatusType.CLOSED)]
    public void AccountStatusType_EnumMembers_VerifyConstantValues(int constValue, AccountStatusType enumMember)
    {
        Assert.Equal(constValue, (int)enumMember);
    }
}