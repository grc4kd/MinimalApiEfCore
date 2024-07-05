using Domain.Accounts.Responses;

namespace Api.Responses;

public class DepositResponse(int customerId, int accountId, decimal balance, bool succeeded) : IDepositResponse
{
    public int CustomerId { get; set; } = customerId;
    public int AccountId { get; set; } = accountId;
    public decimal Balance { get; set; } = balance;
    public bool Succeeded { get; set; } = succeeded;
}