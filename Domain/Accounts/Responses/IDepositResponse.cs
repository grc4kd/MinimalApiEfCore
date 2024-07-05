namespace Domain.Accounts.Responses;

public interface IDepositResponse : IAccountResponse
{
    public bool Succeeded { get; }
}