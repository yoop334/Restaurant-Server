using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Mappers;
using Model.ViewModels.User;
using RestaurantServer.Filters;

namespace RestaurantServer.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("")]
    [AuthorizationFilter]
    public async Task<IActionResult> GetByIdAsync()
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
            return Unauthorized();
        
        var user = await _userService.GetByIdAsync((int)userId);
        if (user == null)
            return NotFound();

        return Ok(
            user.ToViewModel()
        );
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Add([FromBody] UserCreationViewModel userCreationViewModel)
    {
        var addedUser = await _userService.AddAsync(userCreationViewModel.ToEntity());
        if (addedUser == null) return BadRequest("Username already exists!");

        return Ok(addedUser.ToViewModel());
    }

    [HttpPut]
    [Route("")]
    [AuthorizationFilter]
    public async Task<IActionResult> Update([FromBody] UserUpdateViewModel userUpdateViewModel)
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateAsync(userUpdateViewModel, (int)userId);

        if (result == false)
        {
            return NotFound(false);
        }

        return Ok(true);
    }
}