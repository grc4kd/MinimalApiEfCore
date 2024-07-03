using Api.Errors;

namespace Api.Responses;

public class InsufficientFundsResponse : IAccountResponse
{
    private static readonly AccountErrorFeature _error = new() 
        { AccountError = AccountErrorType.InsufficientFundsError };
    public bool IsValid => false;
    public AccountErrorFeature Error => _error;
}
