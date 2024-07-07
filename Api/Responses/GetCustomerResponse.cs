using Domain.Accounts.Data;

namespace Api.Responses;

public record GetCustomerResponse
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public ICollection<Account> Accounts = Array.Empty<Account>();

    public GetCustomerResponse(int customerId, string name)
    {
        CustomerId = customerId;
        Name = name;
    }

    public GetCustomerResponse(int customerId, string name, ICollection<Account> accounts)
    {
        CustomerId = customerId;
        Name = name;
        Accounts = accounts;
    }
}


