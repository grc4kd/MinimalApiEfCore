using Api.Validators;
using Domain.Accounts.Requests;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class AccountTransactionActionFilterService(Settings settings) : IAsyncActionFilter
{
    private readonly AccountTransactionRequestValidator transactionRequestValidator =
        new(settings.MaxDepositAmount, settings.MaxWithdrawalAmount, settings.CurrencyUnitScale);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is IAccountTransactionRequest accountTransactionRequest)
            {
                var validationResult = await transactionRequestValidator.ValidateAsync(accountTransactionRequest);

                if (!validationResult.IsValid)
                {
                    FluentValidationResultModelStateAdapter.ValidationResultToModelStateAsync(context, validationResult);
                    return;
                }
            }
        }

        await next();
    }
}