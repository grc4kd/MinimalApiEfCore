namespace Api.Errors;

public class AccountErrorOptions
{
    public static Action<ProblemDetailsContext> CustomizeProblemDetails => (context) =>
        {
            var accountErrorFeature = context.HttpContext.Features.Get<AccountErrorFeature>();

            if (accountErrorFeature is not null)
            {
                (string Detail, string Type) details = accountErrorFeature.AccountError switch
                {
                    AccountErrorType.InsufficientFundsError => ("The account has insufficient funds.", "https://en.wikipedia.org/wiki/Dishonoured_cheque"),
                    _ => ("Account Error", "Account Error")
                };

                context.ProblemDetails.Type = details.Type;
                context.ProblemDetails.Title = "Insufficient Funds";
                context.ProblemDetails.Detail = details.Detail;
            }
        };
}

