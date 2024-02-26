using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

using Core.Domain.Exceptions;
using Cards.WebAPI.Models.DTOs.Responses;

namespace Cards.WebAPI.Models.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<ExceptionFilter> logger;

    public ExceptionFilter(IWebHostEnvironment env, ILogger<ExceptionFilter> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult), context.Exception, message: context.Exception.Message);

        if (context.Exception.GetType() == typeof(GenericException))
        {
            GenericException genericException = (GenericException)context.Exception;

            ResponseDto<object> responseObject = new()
            {
                Message = genericException.UserMessage!
            };

            context.Result = new HttpActionResult(responseObject, (int)genericException.StatusCode);
            context.HttpContext.Response.StatusCode = (int)genericException.StatusCode;
        }
        else
        {
            string genericMessage = "Sorry, your request could not be competed. If problem persists, please contact us(support@cards.com) for assistance";
            
            if (env.IsDevelopment())
            {
                genericMessage = $"{context.Exception.Message} | {context.Exception.StackTrace}";
            }

            ResponseDto<object> responseDto = new()
            {
                Message = genericMessage
            };

            context.Result = new HttpActionResult(responseDto, (int)HttpStatusCode.InternalServerError);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.ExceptionHandled = true;
    }        
}

