using Api.Requests;
using Api.Validators;
using Microsoft.AspNetCore.Mvc.Filters;

using Adapter = Api.Validators.FluentValidationResultModelStateAdapter;

namespace Api.Filters;

public class AccountActionFilterService(Settings settings) : IAsyncActionFilter
{
    private readonly OpenAccountRequestValidator openAccountRequestValidator = new(settings.MinInitialDepositAmount);
    private readonly WithdrawalRequestValidator withdrawalRequestValidator = new();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is OpenAccountRequest openAccountRequest)
            {
                var validationResult = await openAccountRequestValidator.ValidateAsync(openAccountRequest);
                if (!validationResult.IsValid)
                {
                    Adapter.ValidationResultToModelStateAsync(context, validationResult);
                    return;
                }
            }

            if (request is WithdrawalRequest withdrawalRequest)
            {
                var validationResult = await withdrawalRequestValidator.ValidateAsync(withdrawalRequest);
                if (!validationResult.IsValid)
                {
                    Adapter.ValidationResultToModelStateAsync(context, validationResult);
                    return;
                }
            }
        }

        await next();
    }
}