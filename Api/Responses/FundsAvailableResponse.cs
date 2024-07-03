using Api.Errors;

namespace Api.Responses;

public class FundsAvailableResponse : IAccountResponse
{
    private static readonly AccountErrorFeature Empty = new();
    
    public bool IsValid => true;

    public AccountErrorFeature Error => Empty;
}