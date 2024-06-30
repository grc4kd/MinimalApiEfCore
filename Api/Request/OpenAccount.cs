using Api.Data;

namespace Api.Request;

public class OpenAccount(int customerId, AccountType accountType, double initialDeposit)
{
    public int CustomerId { get; } = customerId;
    public AccountType AccountType { get; } = accountType;
    public double InitialDeposit { get; } = initialDeposit;
}