using Domain.Accounts.Data;

namespace Domain.Accounts.Requests;

public interface IOpenAccountRequest
{
    public int CustomerId { get; }
    public AccountType AccountType { get; }
    public decimal InitialDeposit { get; }
}
