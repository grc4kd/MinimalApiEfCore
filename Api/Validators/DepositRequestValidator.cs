using Api.Request;
using FluentValidation;

namespace Api.Validators;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator() {
        RuleFor(d => d.Amount).GreaterThanOrEqualTo(0.01m);
    }
}