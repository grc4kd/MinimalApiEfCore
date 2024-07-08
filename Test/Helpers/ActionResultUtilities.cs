using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Test.Helpers;

public class ActionResultUtilities
{
    public static (TResult, TResponse, HttpStatusCode) ConvertActionResultToObjects<TResult, TResponse>(IActionResult actionResult)
    {
        const string valuePropertyName = "Value";
        const string statusCodePropertyName = "StatusCode";

        TResult result = (TResult)actionResult;
        var getValueMethod = typeof(TResult).GetProperty(valuePropertyName)?.GetMethod;
        var resultValueObject = getValueMethod?.Invoke(result, null);
        
        if (resultValueObject == null)
        {
            Assert.Fail($"Couldn't get {valuePropertyName} of {typeof(TResult)} from {typeof(IActionResult)}.");
        }

        TResponse response = (TResponse)resultValueObject;
        var getStatusCodeMethod = typeof(TResult).GetProperty("StatusCode")?.GetMethod;
        var responseStatusCodeObject = getStatusCodeMethod?.Invoke(result, null);

        if (responseStatusCodeObject == null)
        {
            Assert.Fail($"Couldn't get {statusCodePropertyName} from {typeof(TResult)}");
        }

        HttpStatusCode httpStatusCode = (HttpStatusCode)responseStatusCodeObject;
        Assert.True(Enum.IsDefined(httpStatusCode));

        return (result, response, httpStatusCode);
    }
}