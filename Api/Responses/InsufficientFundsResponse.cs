namespace Api.Responses;

public class InsufficientFundsResponse : IAccountResponse
{
    public bool IsValid => false;
    public string ErrorMessage => "Account has insufficient funds.";
}
