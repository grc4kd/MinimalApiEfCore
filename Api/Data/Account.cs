using Api.Responses;

namespace Api.Data;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public AccountStatus AccountStatus { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }

    public IAccountResponse CheckBalanceBeforeWithdrawal(decimal amountToWithdrawal)
    {
        if (Balance < amountToWithdrawal) {
            return new InsufficientFundsResponse();
        }

        return new FundsAvailableResponse();
    }

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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, Balance);

        Balance -= amount;
    }
}
