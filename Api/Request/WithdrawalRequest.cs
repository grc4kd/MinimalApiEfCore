using System.ComponentModel.DataAnnotations;

namespace Api.Request;

public class WithdrawalRequest(int customerId, int accountId, decimal amount)
{
    public int CustomerId { get; } = customerId;
    public int AccountId { get; } = accountId;

    [Range(0.01, (double)decimal.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal Amount { get; } = amount;
}