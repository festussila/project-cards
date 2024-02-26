using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Cards.WebAPI.Models.DTOs.Responses;

namespace Cards.WebAPI.Models.Filters;

public class ModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            ResponseDto<object> responseObject = new()
            {
                Message = string.Join(" | ", context.ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage))
            };

            context.Result = new BadRequestObjectResult(responseObject);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}