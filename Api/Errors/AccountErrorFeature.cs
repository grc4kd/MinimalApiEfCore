namespace Api.Errors;

public class AccountErrorFeature
{
    public AccountErrorType AccountError { get; set; }
}

public enum AccountErrorType
{
    InsufficientFundsError
}