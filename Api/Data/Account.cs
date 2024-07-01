

namespace Api.Data;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public AccountStatus AccountStatus { get; set; }
    public decimal Balance { get; set; }

    public void Close()
    {
        AccountStatus = AccountStatus.CLOSED;
    }

    public void MakeDeposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Balance += amount;
    }

    public void MakeWithdrawal(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Balance -= amount;
    }
}