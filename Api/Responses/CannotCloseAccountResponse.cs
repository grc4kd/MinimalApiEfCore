using Domain.Accounts.Responses;

namespace Api.Responses;

public class AccountHasFundedBalanceResponse(int accountId) : IAccountResponse, ICloseAccountResponse
{
    public int AccountId => accountId;
    public bool Succeeded => false;
}