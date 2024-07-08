using FluentValidation.Results;
using Domain.Accounts.Responses;

namespace Api.Responses;

public class AccountTransactionValidationResponse(ValidationResult validationResult) : IAccountTransactionResponse
{
    public bool Succeeded { get; } = validationResult.IsValid;
    public ValidationResult ValidationResult { get; } = validationResult;
}
