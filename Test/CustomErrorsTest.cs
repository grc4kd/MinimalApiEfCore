using Api.Errors;

namespace Test;

public class CustomErrorsTest
{
    [Fact]
    public void AccountErrors_VerifyDatatype()
    {
        Assert.IsType<AccountErrorType>(new AccountErrorFeature().AccountError);
    }
}