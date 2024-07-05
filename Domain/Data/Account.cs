namespace Domain.Data;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public AccountStatus AccountStatus { get; set; } = null!;
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }

    public void Close()
    {
        if (AccountStatus.AccountStatusType == AccountStatusType.CLOSED)
        {
            throw new InvalidOperationException("Account has already been closed.");
        }

        AccountStatus = new AccountStatus(AccountStatusType.CLOSED);
    }

    public void MakeDeposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Balance += amount;
    }

    public void MakeWithdrawal(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, Balance);

        Balance -= amount;
    }
}
