using Domain.Accounts.Responses;

namespace Api.Responses;

public class OpenAccountResponse(int customerId, int accountId, bool succeeded)
    : IAccountResponse, ICustomerResponse, IOpenAccountResponse
{
    public int CustomerId { get; set; } = customerId;
    public int AccountId { get; set; } = accountId;
    public bool Succeeded { get; set; } = succeeded;
}