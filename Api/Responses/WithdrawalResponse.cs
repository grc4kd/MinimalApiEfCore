using Domain.Accounts.Responses;

namespace Api.Responses;

public class WithdrawalResponse(int customerId, int accountId, decimal balance, bool succeeded)
    : IAccountTransactionResponse
{
    public int CustomerId { get; } = customerId;
    public int AccountId { get; } = accountId;
    public decimal Balance { get; } = balance;
    public bool Succeeded { get; } = succeeded;
}