namespace Api.Responses;

public class FundsAvailableResponse : IAccountResponse
{
    public bool IsValid => true;

    public string ErrorMessage => string.Empty;
}