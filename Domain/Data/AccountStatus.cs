namespace Domain.Data;

public class AccountStatus(AccountStatusType accountStatusType)
{
    public AccountStatusType AccountStatusType { get; init; } = accountStatusType;
}

public enum AccountStatusType
{
    OPEN,
    CLOSED
}