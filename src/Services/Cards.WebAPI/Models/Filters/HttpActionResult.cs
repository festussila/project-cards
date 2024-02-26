using Microsoft.AspNetCore.Mvc;

namespace Cards.WebAPI.Models.Filters;

public class HttpActionResult : IActionResult
{
    private readonly object message;
    private readonly int statusCode;

    public HttpActionResult(object message, int statusCode)
    {
        this.message = message;
        this.statusCode = statusCode;
    }

    async Task IActionResult.ExecuteResultAsync(ActionContext context)
    {
        ObjectResult objectResult = new(message)
        {
            StatusCode = statusCode
        };

        await objectResult.ExecuteResultAsync(context);
    }
}