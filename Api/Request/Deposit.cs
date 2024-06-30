namespace Api.Request;

public class Deposit(int customerId, int accountId, double amount)
{
    public int CustomerId { get; } = customerId;
    public int AccountId { get; } = accountId;
    public double Amount { get; } = amount;
}