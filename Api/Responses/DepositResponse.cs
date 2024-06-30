namespace Api.Responses;

public class DepositResponse(int customerId, int accountId, double balance, bool succeeded)
{
    public int CustomerId { get; set; } = customerId;
    public int AccountId { get; set; } = accountId;
    public double Balance { get; set; } = balance;
    public bool Succeeded { get; set; } = succeeded;
}