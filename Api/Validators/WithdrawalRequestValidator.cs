using Api.Request;
using FluentValidation;

namespace Api.Validators;

public class WithdrawalRequestValidator : AbstractValidator<WithdrawalRequest>
{
    public WithdrawalRequestValidator() {
        RuleFor(w => w.Amount).GreaterThanOrEqualTo(0.01m);
    }
}