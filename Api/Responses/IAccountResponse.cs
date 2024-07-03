using Api.Errors;

namespace Api.Responses;

public interface IAccountResponse
{
    public bool IsValid { get; }
    public AccountErrorFeature Error { get; }
}