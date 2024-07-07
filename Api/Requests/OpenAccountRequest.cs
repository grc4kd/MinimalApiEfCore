using Domain.Accounts.Data;
using Domain.Accounts.Requests;

namespace Api.Requests;

public class OpenAccountRequest(int customerId, AccountType accountType, decimal initialDeposit)
    : IOpenAccountRequest
{
    public int CustomerId => customerId;
    public AccountType AccountType => accountType;

    public decimal InitialDeposit => initialDeposit;
}