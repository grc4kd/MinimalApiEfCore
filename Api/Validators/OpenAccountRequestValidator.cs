using Api.Request;
using FluentValidation;

namespace Api.Validators;

public class OpenAccountRequestValidator : AbstractValidator<OpenAccountRequest>
{
    private const decimal DefaultMinInitialDepositAmount = 100;

    public OpenAccountRequestValidator() {
        Initialize(DefaultMinInitialDepositAmount);
    }

    public OpenAccountRequestValidator(decimal minInitialDepositAmount) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minInitialDepositAmount);

        Initialize(minInitialDepositAmount);
    }

    private void Initialize(decimal minInitialDepositAmount)
    {
        RuleFor(o => o.InitialDeposit).GreaterThanOrEqualTo(minInitialDepositAmount);
    }
}