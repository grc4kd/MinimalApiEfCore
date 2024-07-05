namespace Domain.Accounts.Responses;

public interface IOpenAccountResponse : ICustomerResponse
{
    public bool Succeeded { get; }
}