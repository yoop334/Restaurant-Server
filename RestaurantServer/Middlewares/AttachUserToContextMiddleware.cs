using System.IdentityModel.Tokens.Jwt;
using Repository;

namespace RestaurantServer.Middlewares;

public class AttachUserToContextMiddleware
{
    private readonly RequestDelegate _next;

    public AttachUserToContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, DataContext dataContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null) AttachUserToContext(context, dataContext, token);

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, DataContext dataContext, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
        var userId = int.Parse(jwtToken.Subject);
        context.Items["UserId"] = userId;
    }
}