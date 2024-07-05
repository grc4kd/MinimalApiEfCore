using Domain.Accounts.Requests;
using Microsoft.AspNetCore.Mvc.Filters;

using Adapter = Api.Validators.FluentValidationResultModelStateAdapter;
using Api.Validators;

namespace Api.Filters;

public class CurrencyActionFilterService(Settings settings) : IAsyncActionFilter
{
    private readonly AccountTransactionRequestValidator currencyAmountRequestValidator =
        new(settings.MaxDepositAmount, settings.MaxWithdrawalAmount);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is IAccountTransactionRequest accountTransactionRequest)
            {
                var validationResult = await currencyAmountRequestValidator.ValidateAsync(accountTransactionRequest);

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