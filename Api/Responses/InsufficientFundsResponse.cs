using Domain.Accounts.Responses;

namespace Api.Responses;

public class InsufficientFundsResponse(int accountId) : IAccountTransactionResponse
{
    public int AccountId => accountId;
    public bool Succeeded => false;
}
