using System.ComponentModel.DataAnnotations;
using Domain.Accounts.Requests;

namespace Api.Requests;

public class WithdrawalRequest(int customerId, int accountId, decimal amount) : IAccountTransactionRequest
{
    public int CustomerId => customerId;
    public int AccountId => accountId;
    [DataType(DataType.Currency)]
    public decimal Amount => amount;
}