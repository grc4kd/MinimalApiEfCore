namespace Api.Responses;

public class DepositResponse(int customerId, int accountId, decimal balance, bool succeeded)
{
    public int CustomerId { get; set; } = customerId;
    public int AccountId { get; set; } = accountId;
    public decimal Balance { get; set; } = balance;
    public bool Succeeded { get; set; } = succeeded;
}