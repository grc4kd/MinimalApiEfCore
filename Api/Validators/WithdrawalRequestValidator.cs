using FluentValidation;
using Api.Requests;

namespace Api.Validators;

public class WithdrawalRequestValidator : AbstractValidator<WithdrawalRequest>
{
    public WithdrawalRequestValidator(Settings settings)
    {
        decimal minimumWithdrawalAmount = 1m * (decimal)Math.Pow(0.1, settings.CurrencyUnitScale);
        RuleFor(w => w.Amount).GreaterThanOrEqualTo(minimumWithdrawalAmount);
    }
}