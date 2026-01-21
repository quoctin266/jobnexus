using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace JobNexus.Helpers.Filters
{
    public class ResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                int statusCode = objectResult.StatusCode ?? 200;

                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

                var attribute = actionDescriptor?.MethodInfo
                .GetCustomAttribute(typeof(ResponseMessageAttribute), false);

                // Only wrap successful responses
                if (statusCode >= 200 && statusCode < 300)
                {
                    context.Result = new ObjectResult(new
                    {
                        statusCode,
                        message = (attribute as ResponseMessageAttribute)?.Message,
                        data = objectResult.Value
                    })
                    {
                        StatusCode = statusCode
                    };
                }

                if (statusCode >= 400 && statusCode < 500)
                {
                    context.Result = new ObjectResult(new ApiErrorResponse()
                    {
                        statusCode = statusCode,
                        message = (objectResult.Value as ErrorResponse)?.Messages ?? [],
                        error = (objectResult.Value as ErrorResponse)?.Error ?? ""
                    })
                    {
                        StatusCode =statusCode
                    };
                }
            }

        }
    }
}
