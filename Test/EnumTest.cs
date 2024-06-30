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
}