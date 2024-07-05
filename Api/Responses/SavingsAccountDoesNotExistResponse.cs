using Domain.Accounts.Responses;

namespace Api.Responses;

public class SavingsAccountDoesNotExistResponse(int customerId) : ICustomerResponse, IOpenAccountResponse
{
    public int CustomerId => customerId;
    public bool Succeeded => false;
}