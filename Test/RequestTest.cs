using Api.Data;
using Api.Request;

namespace Test;

public class RequestTest
{
    [Fact]
    public void OpenAccount_TestRequestDefinition()
    {
        int customerId = 5;
        AccountType accountType = AccountType.Checking;
        double initialDeposit = 100;

        var request = new OpenAccount(customerId, accountType, initialDeposit);

        Assert.Equal(customerId, request.CustomerId);
        Assert.Equal(accountType, request.AccountType);
        Assert.Equal(initialDeposit, request.InitialDeposit);
    }
}
