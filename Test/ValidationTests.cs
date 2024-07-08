using Domain.Accounts.Requests;
using Api;
using Api.Requests;
using Api.Validators;
using FluentValidation.TestHelper;
using Test.TheoryData;

namespace Test;

public class ValidationTests
{
    readonly Settings safeSettings = new()
    {
        MaxDepositAmount = 1_000_000,
        MaxWithdrawalAmount = 1_000_000,
        MinInitialDepositAmount = 0.01m,
        CurrencyUnitScale = 2
    };

    [Theory]
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    public async Task ValidateRequest_WithdrawalRequest_TestValidate(
        int customerId, int accountId, decimal amount
        )
    {
        var request = new WithdrawalRequest(customerId, accountId, amount);
        var validator = new WithdrawalRequestValidator(safeSettings);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible",
        Justification = "xUnit uses public static member fields to pass member data into [Theory] test methods by design")]
    public static AccountTransactionRequestTheoryData CurrencyAmountRequestData = [];

    [Theory]
    [MemberData(nameof(CurrencyAmountRequestData))]
    public async Task ValidateRequest_IAccountTransactionRequest_TestValidate(IAccountTransactionRequest request)
    {
        var validator = new AccountTransactionRequestValidator(safeSettings);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [Theory]
    [InlineData(0, 1, 0, 1, nameof(Settings.MaxDepositAmount))]
    [InlineData(1, 0, 0, 1, nameof(Settings.MaxWithdrawalAmount))]
    [InlineData(1, 1, -1, 1, nameof(Settings.CurrencyUnitScale))]
    [InlineData(1, 1, 0, 0, nameof(Settings.MinInitialDepositAmount))]
    public void Settings_ValidateArguments_CheckExceptions(
        decimal maxDepositAmount, decimal maxWithdrawalAmount, int currencyUnitScale, decimal minInitialDepositAmount,
        string paramName)
    {
        Assert.Throws<ArgumentOutOfRangeException>(paramName, ()
            => new Settings
            {
                MaxDepositAmount = maxDepositAmount,
                MaxWithdrawalAmount = maxWithdrawalAmount,
                CurrencyUnitScale = currencyUnitScale,
                MinInitialDepositAmount = minInitialDepositAmount
            });
    }

    [Fact]
    public async Task Settings_ValidateArguments_CheckStrangeButValidParameters()
    {
        var strangeSettings = new Settings 
        {
            MaxDepositAmount = safeSettings.MaxDepositAmount,
            MaxWithdrawalAmount = 1_000_000.23m,
            CurrencyUnitScale = safeSettings.CurrencyUnitScale,
            MinInitialDepositAmount = safeSettings.MinInitialDepositAmount
        };

        var validator = new AccountTransactionRequestValidator(strangeSettings);

        var normalRequest = new WithdrawalRequest(1, 1, 1_000_000);
        var invalidRequest = new WithdrawalRequest(1, 1, 10_000_000);

        var normalResult = await validator.TestValidateAsync(normalRequest);
        var invalidResult = await validator.TestValidateAsync(invalidRequest);

        normalResult.ShouldNotHaveAnyValidationErrors();
        invalidResult.ShouldHaveValidationErrorFor(nameof(WithdrawalRequest.Amount));
    }
}