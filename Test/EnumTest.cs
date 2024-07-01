using Api.Data;

namespace Test;

public class EnumTest
{
    [Fact]
    public void AccountType_TestEnumValues()
    {
        Assert.Equal((AccountType)1, AccountType.Checking);
        Assert.Equal((AccountType)2, AccountType.Savings);
    }

    [Fact]
    public void AccountStatus_TestEnumValues()
    {
        Assert.Equal((AccountStatus)0, AccountStatus.OPEN);
        Assert.Equal((AccountStatus)1, AccountStatus.CLOSED);
    }
}
