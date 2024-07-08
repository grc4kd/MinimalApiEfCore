using FluentValidation.Results;
using Domain.Accounts.Responses;

namespace Api.Responses;

public class OpenAccountValidationResponse(int customerId, ValidationResult validationResult) : IOpenAccountResponse
{
    public bool Succeeded { get; } = validationResult.IsValid;
    public int CustomerId { get; } = customerId;
    public ValidationResult ValidationResult { get; } = validationResult;
}
