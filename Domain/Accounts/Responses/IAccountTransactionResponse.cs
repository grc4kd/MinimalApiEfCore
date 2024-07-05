namespace Domain.Accounts.Responses;

public interface IAccountTransactionResponse : IAccountResponse
{
    public bool Succeeded { get; }
    public decimal Balance { get; }
}