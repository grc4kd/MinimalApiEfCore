using Domain.Accounts.Requests;
using FluentValidation;
using Api.Requests;

namespace Api.Validators;

public class AccountTransactionRequestValidator : AbstractValidator<IAccountTransactionRequest>
{
    public AccountTransactionRequestValidator(Settings settings)
    {
        int DepositPrecision = (int)Math.Log10((double)settings.MaxDepositAmount) + settings.CurrencyUnitScale + 1;
        int WithdrawalPrecision = (int)Math.Log10((double)settings.MaxWithdrawalAmount) + settings.CurrencyUnitScale + 1;

        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.Amount).PrecisionScale(DepositPrecision, settings.CurrencyUnitScale, true).When(r => r is DepositRequest);
        RuleFor(r => r.Amount).PrecisionScale(WithdrawalPrecision, settings.CurrencyUnitScale, true).When(r => r is WithdrawalRequest);
    }
}