using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Account;
using RestaurantServer.Filters;

namespace RestaurantServer.Controllers;


[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [Route("login")]
    [Produces("application/json")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthenticationRequest loginModel)
    {
        IActionResult response = Unauthorized();
        var data = await _accountService.AuthenticateAsync(loginModel.Username, loginModel.Password);

        if (data?.AccessToken != null) response = Ok(data);

        return response;
    }


    [HttpPut]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest refreshTokenModel)
    {
        if (string.IsNullOrEmpty(refreshTokenModel.AccessToken) ||
            string.IsNullOrEmpty(refreshTokenModel.RefreshToken))
            return Unauthorized();

        var response = await _accountService.RefreshAsync(refreshTokenModel);

        if (response == null ||
            string.IsNullOrEmpty(response.AccessToken) ||
            string.IsNullOrEmpty(response.RefreshToken))
            return Unauthorized();

        return Ok(response);
    }

    [HttpGet]
    [Route("check-token")]
    public async Task<IActionResult> IsTokenValid()
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null) return Unauthorized();

        var response = await _accountService.CheckToken((int)userId);

        return Ok(response);
    }

    [HttpPost]
    [Route("logout")]
    [Produces("application/json")]
    [AuthorizationFilter]
    public async Task<IActionResult> LogoutAsync()
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null) return Unauthorized();

        return Ok(
            await _accountService.RevokeRefreshToken((int)userId)
        );
    }
}