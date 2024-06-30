namespace Api.Responses;

public class WithdrawalResponse(int customerId, int accountId, double balance, bool succeeded)
{
    public int CustomerId { get; } = customerId;
    public int AccountId { get; } = accountId;
    public double Balance { get; } = balance;
    public bool Succeeded { get; } = succeeded;
}