using Api.Request;
using FluentValidation;

namespace Api.Validators;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    private const decimal DefaultMinDepositAmount = 0.01m;

    public DepositRequestValidator() {
        Initialize(DefaultMinDepositAmount);
    }

    public DepositRequestValidator(decimal minDepositAmount) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minDepositAmount);

        Initialize(minDepositAmount);
    }

    private void Initialize(decimal minDepositAmount)
    {
        RuleFor(d => d.Amount).GreaterThanOrEqualTo(minDepositAmount);
    }
}