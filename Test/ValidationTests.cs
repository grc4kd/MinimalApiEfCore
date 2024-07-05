using Domain.Data;
using Domain.Accounts.Requests;
using Api;
using Api.Requests;
using Api.Validators;
using Api.Filters;
using FluentValidation.TestHelper;
using Test.TheoryData;

namespace Test;

public class ValidationTests
{
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [Theory]
    public async Task ValidateRequest_WithdrawalRequest_TestValidate(
        int customerId, int accountId, decimal amount
        )
    {
        var request = new WithdrawalRequest(customerId, accountId, amount);
        var validator = new WithdrawalRequestValidator();

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible",
        Justification = "xUnit uses public static member fields to pass member data into [Theory] test methods by design")]
    public static CurrencyAmountRequestTheoryData CurrencyAmountRequestData = [];

    [Theory]
    [MemberData(nameof(CurrencyAmountRequestData))]
    public async Task ValidateRequest_IAccountTransactionRequest_TestValidate(IAccountTransactionRequest request)
    {
        var validator = new AccountTransactionRequestValidator();

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [Fact]
    public void AccountActionFilterService_MinDepositAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 1_000_000, MaxWithdrawalAmount = 1_000_000, MinInitialDepositAmount = 0 };

        Assert.Throws<ArgumentOutOfRangeException>("minInitialDepositAmount", () => new AccountActionFilterService(settings));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxDepositAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 0, MaxWithdrawalAmount = 1_000_000, MinInitialDepositAmount = 1_000_000 };

        Assert.Throws<ArgumentOutOfRangeException>("maxDepositAmount", () => new CurrencyActionFilterService(settings));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxWithdrawalAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 1_000_000, MaxWithdrawalAmount = 0, MinInitialDepositAmount = 1_000_000 };

        Assert.Throws<ArgumentOutOfRangeException>("maxWithdrawalAmount", () => new CurrencyActionFilterService(settings));
    }

    [Fact]
    public async Task OpenAccountRequestValidator_TestDefaults()
    {
        var openAccountRequest = new OpenAccountRequest(customerId: 1, AccountType.Savings, initialDeposit: 20);
        var openAccountRequestValidator = new OpenAccountRequestValidator();

        var result = await openAccountRequestValidator.TestValidateAsync(openAccountRequest);

        result.ShouldHaveValidationErrorFor(r => r.InitialDeposit);
    }
}