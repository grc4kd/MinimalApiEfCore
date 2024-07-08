using FluentValidation;
using Domain.Accounts.Requests;

namespace Api.Validators;

public class OpenAccountRequestValidator : AbstractValidator<IOpenAccountRequest>
{
    public OpenAccountRequestValidator(Settings settings)
    {
        RuleFor(o => o.InitialDeposit).GreaterThanOrEqualTo(settings.MinInitialDepositAmount);
    }
}