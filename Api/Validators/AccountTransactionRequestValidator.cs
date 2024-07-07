using Domain.Accounts.Requests;
using FluentValidation;
using Api.Requests;

namespace Api.Validators;

public class AccountTransactionRequestValidator : AbstractValidator<IAccountTransactionRequest>
{
    private const decimal DefaultMaxDepositAmount = 1_000_000_000;
    private const decimal DefaultMaxWithdrawalAmount = 1_000_000_000;
    private const int DefaultCurrencyUnitScale = 2;

    public AccountTransactionRequestValidator() => Initialize(DefaultMaxDepositAmount, DefaultMaxWithdrawalAmount, DefaultCurrencyUnitScale);

    public AccountTransactionRequestValidator(decimal MaxDepositAmount, decimal MaxWithdrawalAmount, int CurrencyUnitScale)
        => Initialize(MaxDepositAmount, MaxWithdrawalAmount, CurrencyUnitScale);

    private void Initialize(decimal MaxDepositAmount, decimal MaxWithdrawalAmount, int CurrencyUnitScale)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxDepositAmount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxWithdrawalAmount);
        ArgumentOutOfRangeException.ThrowIfNegative(CurrencyUnitScale);

        int DepositPrecision = (int)Math.Log10((double)MaxDepositAmount) + CurrencyUnitScale;
        int WithdrawalPrecision = (int)Math.Log10((double)MaxWithdrawalAmount) + CurrencyUnitScale;

        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.Amount).PrecisionScale(DepositPrecision, CurrencyUnitScale, true).When(r => r is DepositRequest);
        RuleFor(r => r.Amount).PrecisionScale(WithdrawalPrecision, CurrencyUnitScale, true).When(r => r is WithdrawalRequest);
    }
}