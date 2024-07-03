using Api;
using Api.Filters;
using Api.Request;
using Api.Validators;
using FluentValidation.TestHelper;
using Test.TheoryData;

namespace Test;

public class ValidationTest
{
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [InlineData(1, 1, 99.99)]
    [Theory]
    public async Task ValidateRequest_DepositRequest_TestValidate(
        int customerId, int accountId, decimal amount
    )
    {
        var request = new DepositRequest(customerId, accountId, amount);
        var validator = new DepositRequestValidator(minDepositAmount: 100.00m);

        var result = await validator.TestValidateAsync(request);
        
        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

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
    public async Task ValidateRequest_ICurrencyAmountRequest_TestValidate(ICurrencyAmountRequest request)
    {
        var validator = new CurrencyAmountRequestValidator();

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [Fact]
    public void AccountActionFilterService_MinDepositAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 1_000_000, MaxWithdrawalAmount = 1_000_000, MinDepositAmount = 0 };

        Assert.Throws<ArgumentOutOfRangeException>("minDepositAmount", () => new AccountActionFilterService(settings));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxDepositAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 0, MaxWithdrawalAmount = 1_000_000, MinDepositAmount = 1_000_000 };
    
        Assert.Throws<ArgumentOutOfRangeException>("maxDepositAmount", () => new CurrencyActionFilterService(settings));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxWithdrawalAmount_TestConfigException()
    {
        Settings settings = new() { MaxDepositAmount = 1_000_000, MaxWithdrawalAmount = 0, MinDepositAmount = 1_000_000 };
    
        Assert.Throws<ArgumentOutOfRangeException>("maxWithdrawalAmount", () => new CurrencyActionFilterService(settings));
    }
}