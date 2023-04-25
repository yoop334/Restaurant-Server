using System.Net;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantServer.Filters;

public class AuthorizationFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = ((string)context.HttpContext.Request.Headers["Authorization"])?.Split(" ")[1];
        var authorizationHelper =
            (IAuthorizationHelper)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationHelper));

        if (token != null && authorizationHelper.IsAccessTokenValid(token))
            await next.Invoke();
        else
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}