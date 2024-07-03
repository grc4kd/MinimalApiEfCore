using Api.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class CurrencyActionFilter : ActionFilterAttribute
{
    const decimal DefaultMaxDepositAmount = 1_000_000_000;
    const decimal DefaultMaxWithdrawalAmount = 1_000_000_000;

    public static decimal MaxDepositAmount { get; set; } = DefaultMaxDepositAmount;
    public static decimal MaxWithdrawalAmount { get; set; } = DefaultMaxWithdrawalAmount;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is ICurrencyAmountRequest currencyAmountRequest)
            {
                if (currencyAmountRequest.Amount.Scale > 2)
                {
                    context.ModelState.AddModelError(nameof(currencyAmountRequest.Amount),
                        "Amount must use a maximum of two decimal places.");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }

            if (request is DepositRequest depositRequest) 
            {
                if (depositRequest.Amount > MaxDepositAmount)
                {
                    context.ModelState.AddModelError(nameof(depositRequest.Amount),
                        $"Amount must be less than or equal to maximum deposit amount: {MaxDepositAmount}");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }

            if (request is WithdrawalRequest withdrawalRequest) 
            {
                if (withdrawalRequest.Amount > MaxWithdrawalAmount)
                {
                    context.ModelState.AddModelError(nameof(withdrawalRequest.Amount),
                        $"Amount must be less than or equal to maximum withdrawal amount: {MaxWithdrawalAmount}");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
        }
    }
}