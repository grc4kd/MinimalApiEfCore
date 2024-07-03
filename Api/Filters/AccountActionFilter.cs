using Api.Data;
using Api.Request;
using Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class AccountActionFilter : ActionFilterAttribute
{
    private readonly DepositRequestValidator depositRequestValidator = new();
    private readonly WithdrawalRequestValidator withdrawalRequestValidator = new();

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var request))
        {
            if (request is DepositRequest depositRequest) 
            {
                var validationResult = await depositRequestValidator.ValidateAsync(depositRequest);
                if (!validationResult.IsValid) {
                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }
            }

            if (request is WithdrawalRequest withdrawalRequest) 
            {
                var validationResult = await withdrawalRequestValidator.ValidateAsync(withdrawalRequest);
                if (!validationResult.IsValid) {
                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }
            }
        }

        await next();
    }
}