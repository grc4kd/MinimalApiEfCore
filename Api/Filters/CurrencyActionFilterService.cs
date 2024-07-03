using Api.Request;
using Api.Validators;
using Microsoft.AspNetCore.Mvc.Filters;

using Adapter = Api.Validators.FluentValidationResultModelStateAdapter;

namespace Api.Filters;

public class CurrencyActionFilterService(Settings settings) : IAsyncActionFilter
{
    private readonly CurrencyAmountRequestValidator currencyAmountRequestValidator = 
        new(settings.MaxDepositAmount, settings.MaxWithdrawalAmount);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is ICurrencyAmountRequest currencyAmountRequest)
            {
                var validationResult = await currencyAmountRequestValidator.ValidateAsync(currencyAmountRequest);

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