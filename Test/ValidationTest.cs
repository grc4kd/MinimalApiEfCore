using System.ComponentModel.DataAnnotations;
using Api.Request;

namespace Test;

public class ValidationTest
{
    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [Theory]
    public void ValidateRequest_DepositRequest_InvalidPropertyValues(
        int customerId, int accountId, decimal amount
    )
    {
        var request = new DepositRequest(customerId, accountId, amount);
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(request, context, results, validateAllProperties: true);

        Assert.False(valid, $"Expected request to be invalid. CustomerId: {customerId}, AccountId: {accountId}, Amount: {amount}");
    }

    [InlineData(1, 1, -1)]
    [InlineData(1, 1, 0)]
    [Theory]
    public void ValidateRequest_WithdrawalRequest_InvalidPropertyValues(
        int customerId, int accountId, decimal amount
    )
    {
        var request = new WithdrawalRequest(customerId, accountId, amount);
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(request, context, results, validateAllProperties: true);

        Assert.False(valid, $"Expected request to be invalid. CustomerId: {customerId}, AccountId: {accountId}, Amount: {amount}");
    }
}