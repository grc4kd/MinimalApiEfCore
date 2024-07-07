using Domain.Accounts.Data;

namespace Test;

public class EnumTests
{
    [Fact]
    public void AccountType_TestEnumValues()
    {
        Assert.Equal(1, (int)AccountType.Checking);
        Assert.Equal(2, (int)AccountType.Savings);
    }

    [Fact]
    public void AccountStatusType_TestEnumValues()
    {
        Assert.Equal(0, (int)AccountStatusType.OPEN);
        Assert.Equal(1, (int)AccountStatusType.CLOSED);
    }
}
