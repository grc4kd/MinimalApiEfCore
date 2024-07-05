using Domain.Accounts.Responses;

namespace Api.Responses;

public class CustomerNotFoundResponse(int customerId) : ICustomerResponse, IOpenAccountResponse
{
    public int CustomerId => customerId;
    public bool Succeeded => false;
}