using System.Net;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.Enums;

namespace RestaurantServer.Filters;

public class AuthorizationFilter : ActionFilterAttribute
{
    private string _allowedRoles;
    
    public AuthorizationFilter()
    {
        _allowedRoles = $"{UserRole.Client},{UserRole.Admin}";
    }
    
    public AuthorizationFilter(string roles)
    {
        _allowedRoles = roles;
    }
    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = ((string)context.HttpContext.Request.Headers["Authorization"])?.Split(" ")[1];
        var authorizationHelper =
            (IAuthorizationHelper)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationHelper));

        if (token != null && authorizationHelper.IsAccessTokenValid(token) &&
             await authorizationHelper.IsUsersRoleAuthorized(token, _allowedRoles))
            await next.Invoke();
        else
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}