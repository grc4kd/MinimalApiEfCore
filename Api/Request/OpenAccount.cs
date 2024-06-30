using Api.Data;

namespace Api.Request;

public class OpenAccount
{
    public int CustomerId { get; }
    public AccountType AccountType { get; }
    public double InitialDeposit { get; }

    public OpenAccount(int customerId, AccountType accountType, double initialDeposit)
    {
        CustomerId = customerId;
        AccountType = accountType;
        InitialDeposit = initialDeposit;
    }
}