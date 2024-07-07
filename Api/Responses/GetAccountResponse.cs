using Domain.Accounts.Data;

namespace Api.Responses;

public record GetAccountResponse
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountStatus { get; set; }
    public string AccountType { get; set; }
    public decimal Balance { get; set; }

    public GetAccountResponse(int accountId, int customerId, string name, 
        AccountStatus accountStatus, AccountType accountType, decimal balance)
        {
            AccountId = accountId;
            CustomerId = customerId;
            Name = name;
            AccountStatus = accountStatus.AccountStatusType.ToString() ?? string.Empty;
            AccountType = accountType.ToString() ?? string.Empty;
            Balance = balance;
        }
}