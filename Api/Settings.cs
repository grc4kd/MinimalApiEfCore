namespace Api;

public sealed class Settings
{
    private decimal maxWithdrawalAmount;
    private decimal maxDepositAmount;
    private int currencyUnitScale;
    private decimal minInitialDepositAmount;

    public required decimal MaxDepositAmount
    {
        get => maxDepositAmount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(MaxDepositAmount));
            maxDepositAmount = value;
        }
    }

    public required decimal MaxWithdrawalAmount
    {
        get => maxWithdrawalAmount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(MaxWithdrawalAmount));
            maxWithdrawalAmount = value;
        }
    }

    public required int CurrencyUnitScale
    {
        get => currencyUnitScale;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(CurrencyUnitScale));
            currencyUnitScale = value;
        }
    }

    public required decimal MinInitialDepositAmount
    {
        get => minInitialDepositAmount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(MinInitialDepositAmount));
            minInitialDepositAmount = value;
        }
    }
}