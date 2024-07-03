using Api.Request;
using Api.Validators;
using Microsoft.AspNetCore.Mvc.Filters;

using Adapter = Api.Validators.FluentValidationResultModelStateAdapter;

namespace Api.Filters;

public class AccountActionFilterService(Settings settings) : IAsyncActionFilter
{
    private readonly DepositRequestValidator depositRequestValidator = new(settings.MinDepositAmount);
    private readonly WithdrawalRequestValidator withdrawalRequestValidator = new();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is DepositRequest depositRequest) 
            {
                var validationResult = await depositRequestValidator.ValidateAsync(depositRequest);
                if (!validationResult.IsValid)
                {
                    Adapter.ValidationResultToModelStateAsync(context, validationResult);
                    return;
                }
            }

            if (request is WithdrawalRequest withdrawalRequest) 
            {
                var validationResult = await withdrawalRequestValidator.ValidateAsync(withdrawalRequest);
                if (!validationResult.IsValid) {
                    Adapter.ValidationResultToModelStateAsync(context, validationResult);
                    return;
                }
            }
        }

        await next();
    }
}