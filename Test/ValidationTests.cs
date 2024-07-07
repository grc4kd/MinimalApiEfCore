using Domain.Accounts.Requests;
using Api;
using Api.Requests;
using Api.Validators;
using Api.Filters;
using FluentValidation.TestHelper;
using Test.TheoryData;
using Domain.Accounts.Data;

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
        decimal invalidSetting = 0;
        
        Assert.Throws<ArgumentOutOfRangeException>(nameof(Settings.MinInitialDepositAmount), ()
            => new AccountActionFilterService(new Settings {
                MaxDepositAmount = 1,
                MaxWithdrawalAmount = 1,
                CurrencyUnitScale = 0,
                MinInitialDepositAmount = invalidSetting
            }));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxDepositAmount_TestConfigException()
    {
        decimal invalidSetting = 0;
        
        Assert.Throws<ArgumentOutOfRangeException>(nameof(Settings.MaxDepositAmount), ()
            => new AccountTransactionActionFilterService(new Settings {
                MaxDepositAmount = invalidSetting,
                MaxWithdrawalAmount = 1,
                CurrencyUnitScale = 0,
                MinInitialDepositAmount = 1
            }));
    }

    [Fact]
    public void CurrencyActionFilterService_MaxWithdrawalAmount_TestConfigException()
    {
        decimal invalidSetting = 0;
        
        Assert.Throws<ArgumentOutOfRangeException>(nameof(Settings.MaxWithdrawalAmount), ()
            => new AccountTransactionActionFilterService(new Settings {
                MaxDepositAmount = 1,
                MaxWithdrawalAmount = invalidSetting,
                CurrencyUnitScale = 0,
                MinInitialDepositAmount = 1
            }));
    }

    [Fact]
    public void CurrencyActionFilterService_CurrencyUnitScale_TestConfigException()
    {
        int invalidSetting = -2;
        
        Assert.Throws<ArgumentOutOfRangeException>(nameof(Settings.CurrencyUnitScale), ()
            => new AccountTransactionActionFilterService(new Settings {
                MaxDepositAmount = 1,
                MaxWithdrawalAmount = 1,
                CurrencyUnitScale = invalidSetting,
                MinInitialDepositAmount = 1
            }));
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