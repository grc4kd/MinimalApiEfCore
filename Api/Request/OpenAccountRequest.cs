using Api.Data;

namespace Api.Request;

public class OpenAccountRequest(int customerId, AccountType accountType, decimal initialDeposit)
{
    public int CustomerId { get; } = customerId;
    public AccountType AccountType { get; } = accountType;
    public decimal InitialDeposit { get; } = initialDeposit;
}