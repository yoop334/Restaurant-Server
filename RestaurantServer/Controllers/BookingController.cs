using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Mappers;
using Model.ViewModels.Booking;
using RestaurantServer.Filters;

namespace RestaurantServer.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    [Route("{date:long}")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult GetAvailableTables([FromRoute] long date)
    {
        return Ok(_bookingService.GetAvailableNrOfPersons(date));
    }

    [HttpGet]
    [Route("{date:long}/{nrOfPersons:int}")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult GetAvailableHours([FromRoute] long date, [FromRoute] int nrOfPersons)
    {
        return Ok(_bookingService.GetAvailableHours(date, nrOfPersons));
    }
    
    [HttpPost]
    [Route("")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult AddBooking([FromBody] BookingCreationViewModel bookingCreationViewModel)
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        var addedBooking = _bookingService.AddBooking(bookingCreationViewModel.ToEntity(), (int)userId);
        return Ok(addedBooking.Result != null);
    }
    
    [HttpGet]
    [Route("")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult GetBookingsForUser()
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var res = _bookingService.GetBookingsForUser((int)userId);
        if (res.Result == null)
        {
            return Ok("Error");
        }
        var bookings = res.Result.Select(booking => booking.ToViewModel())
            .OrderBy(booking => booking.Time);
        return Ok(bookings);
    }

    [HttpDelete]
    [Route("{id:long}")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult DeleteBookingForUser([FromRoute] long id)
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = _bookingService.DeleteBooking(id);

        if (result.Result == false)
        {
            return NotFound();
        }
        
        return Ok(result.Result);
    }
    
    [HttpGet]
    [Route("all/{date:long}")]
    [AuthorizationFilter(roles: "Admin")]
    public IActionResult GetAllBookingsByDate([FromRoute] long date)
    {
        var res = _bookingService.GetAllBookingsByDate(date);
        return Ok(res);
    }

    [HttpGet]
    [Route("first")]
    [AuthorizationFilter(roles: "Client")]
    public IActionResult GetFirstBookingForUser()
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = _bookingService.GetBookingsForUser((int)userId);

        if (result.Result == null)
        {
            return NotFound();
        }

        var booking = result.Result.Select(booking => booking.ToViewModel())
            .Where(booking => booking.Time > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            .MinBy(booking => booking.Time);
        
        if (booking == default)
        {
            return NotFound();
        }
        return Ok(booking);
    }
}