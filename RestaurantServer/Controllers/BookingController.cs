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
    [AuthorizationFilter]
    public IActionResult GetAvailableTables([FromRoute] long date)
    {
        return Ok(_bookingService.GetAvailableNrOfPersons(date));
    }

    [HttpGet]
    [Route("{date:long}/{nrOfPersons:int}")]
    [AuthorizationFilter]
    public IActionResult GetAvailableHours([FromRoute] long date, [FromRoute] int nrOfPersons)
    {
        return Ok(_bookingService.GetAvailableHours(date, nrOfPersons));
    }
    
    [HttpPost]
    [Route("")]
    [AuthorizationFilter]
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
    [AuthorizationFilter]
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
        var bookings = res.Result.Select(booking => booking.ToViewModel());
        return Ok(bookings);
    }
}