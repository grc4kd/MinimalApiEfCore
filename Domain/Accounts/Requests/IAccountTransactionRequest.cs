namespace Domain.Accounts.Requests;

public interface IAccountTransactionRequest
{
    public int CustomerId { get; }
    public int AccountId { get; }
    public decimal Amount { get; }
}
