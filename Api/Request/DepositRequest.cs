using System.ComponentModel.DataAnnotations;

namespace Api.Request;

public class DepositRequest(int customerId, int accountId, decimal amount) : ICurrencyAmountRequest
{
    public int CustomerId { get; } = customerId;
    public int AccountId { get; } = accountId;

    [DataType(DataType.Currency)]
    public decimal Amount { get; } = amount;
}
