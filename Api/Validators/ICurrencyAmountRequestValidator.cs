using Api.Request;
using FluentValidation;

namespace Api.Validators;

public class CurrencyAmountRequestValidator : AbstractValidator<ICurrencyAmountRequest>
{
    private const decimal DefaultMaxDepositAmount = 1_000_000_000.00m;
    private const decimal DefaultMaxWithdrawalAmount = 1_000_000_000.00m;

    public CurrencyAmountRequestValidator() => Initialize(DefaultMaxDepositAmount, DefaultMaxWithdrawalAmount);

    public CurrencyAmountRequestValidator(decimal maxDepositAmount, decimal maxWithdrawalAmount) 
        => Initialize(maxDepositAmount, maxWithdrawalAmount);

    private void Initialize(decimal maxDepositAmount, decimal maxWithdrawalAmount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDepositAmount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWithdrawalAmount);

        int DepositPrecision = (int)Math.Log10((double)maxDepositAmount) + maxDepositAmount.Scale + 1;
        int WithdrawalPrecision = (int)Math.Log10((double)maxWithdrawalAmount) + maxWithdrawalAmount.Scale + 1;

        RuleFor(r => r.Amount).PrecisionScale(DepositPrecision, maxDepositAmount.Scale, true).When(r => r is DepositRequest);
        RuleFor(r => r.Amount).PrecisionScale(WithdrawalPrecision, maxWithdrawalAmount.Scale, true).When(r => r is WithdrawalRequest);
    }
}