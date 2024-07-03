using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Validators;

public static class FluentValidationResultModelStateAdapter
{
    internal static void ValidationResultToModelStateAsync(ActionExecutingContext context, ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            foreach (var validationFailure in validationResult.Errors)
            {
                context.ModelState.TryAddModelError(validationFailure.PropertyName, validationFailure.ErrorMessage);
            }

            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}