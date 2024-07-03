using System.ComponentModel.DataAnnotations;
using Api.Data;
using Api.Request;
using Api.Validators;
using FluentValidation.TestHelper;

namespace Test;

public class ValidationTest
{
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [Theory]
    public async Task ValidateRequest_DepositRequest_InvalidPropertyValues(
        int customerId, int accountId, decimal amount
    )
    {
        var request = new DepositRequest(customerId, accountId, amount);
        var validator = new DepositRequestValidator();

        var result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [Theory]
    public async Task ValidateRequest_WithdrawalRequest_InvalidPropertyValues(
        int customerId, int accountId, decimal amount
        )
    {
        var request = new WithdrawalRequest(customerId, accountId, amount);
        var validator = new WithdrawalRequestValidator();

        var result = await validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }
}