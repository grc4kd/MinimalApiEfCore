using System.ComponentModel.DataAnnotations;

namespace Api.Request;

public class WithdrawalRequest(int customerId, int accountId, decimal amount) : ICurrencyAmountRequest
{
    public int AccountId { get; } = accountId;
    public int CustomerId { get; } = customerId;

    [DataType(DataType.Currency)]
    public decimal Amount { get; } = amount;
}