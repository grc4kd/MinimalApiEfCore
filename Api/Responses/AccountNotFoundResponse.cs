using Domain.Accounts.Responses;

namespace Api.Responses;

public class AccountNotFoundResponse(int accountId) 
    : IAccountResponse, IAccountTransactionResponse, ICloseAccountResponse
{
    public int AccountId => accountId;
    public bool Succeeded => false;
}