namespace Api.Responses;

public interface IAccountResponse
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }
}