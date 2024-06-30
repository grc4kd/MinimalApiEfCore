namespace Api.Request;

public class CloseAccount(int customerId, int accountId)
{
    public int AccountId { get; } = accountId;
    public int CustomerId { get; } = customerId;
}