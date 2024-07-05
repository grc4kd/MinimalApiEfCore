namespace Domain.Accounts.Requests;

public interface ICloseAccountRequest
{
    public int CustomerId { get; }
    public int AccountId { get; }
}