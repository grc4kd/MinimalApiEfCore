using FluentValidation;
using Api.Requests;

namespace Api.Validators;

public class OpenAccountRequestValidator : AbstractValidator<OpenAccountRequest>
{
    private const decimal DefaultMinInitialDepositAmount = 100;

    public OpenAccountRequestValidator()
    {
        Initialize(DefaultMinInitialDepositAmount);
    }

    public OpenAccountRequestValidator(decimal MinInitialDepositAmount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MinInitialDepositAmount);

        Initialize(MinInitialDepositAmount);
    }

    private void Initialize(decimal MinInitialDepositAmount)
    {
        RuleFor(o => o.InitialDeposit).GreaterThanOrEqualTo(MinInitialDepositAmount);
    }
}