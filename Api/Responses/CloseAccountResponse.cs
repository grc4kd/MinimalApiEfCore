namespace Api.Responses;

public class CloseAccountResponse(int customerId, int accountId, bool succeeded)
{
    public int AccountId { get; set; } = accountId;
    public int CustomerId { get; set; } = customerId;
    public bool Succeeded { get; set; } = succeeded;
}
