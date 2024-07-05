using FluentValidation;
using Api.Requests;

namespace Api.Validators;

public class WithdrawalRequestValidator : AbstractValidator<WithdrawalRequest>
{
    public WithdrawalRequestValidator()
    {
        RuleFor(w => w.Amount).GreaterThanOrEqualTo(0.01m);
    }
}