using Api.Data;

namespace Test;

public class ToStringTest
{
    [Fact]
    public void Customer_ToStringTest()
    {
        var customerName = "Carl";

        var customer = new Customer { 
            Id = 1, 
            Name = customerName
        };
        customer.Accounts.Add(
            new Account { 
                Id = 1,
                AccountStatus = AccountStatus.OPEN, 
                AccountType = AccountType.Savings,
                Balance = 100m,
                CustomerId = 1,
            }
        );

        Assert.Contains(customerName, customer.ToString());
        Assert.Contains("00000001", customer.ToString());
    }
}