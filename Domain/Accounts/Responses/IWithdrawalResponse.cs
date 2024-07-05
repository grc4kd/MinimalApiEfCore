namespace Domain.Accounts.Responses;

public interface IWithdrawalResponse : IAccountResponse
{
    public bool Succeeded { get; }
}