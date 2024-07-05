using Domain.Accounts.Requests;

namespace Api.Requests;

public class CloseAccountRequest(int customerId, int accountId) : ICloseAccountRequest
{
    public int CustomerId => customerId;
    public int AccountId => accountId;
}