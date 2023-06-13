using Model.Entities;
using Model.ViewModels;
using Model.ViewModels.Booking;

namespace Model.Mappers;

public static class BookingMapper
{
    public static Booking ToEntity(this BookingCreationViewModel bookingCreationViewModel)
    {
        var booking = new Booking
        {
            Time = DateTimeOffset.FromUnixTimeMilliseconds(bookingCreationViewModel.Date).LocalDateTime.Date + new TimeSpan(bookingCreationViewModel.Time, 0, 0),
            NrOfPersons = bookingCreationViewModel.NrOfPersons
        };

        return booking;
    }
    
    public static BookingViewModel ToViewModel(this Booking booking)
    {
        var bookingViewModel = new BookingViewModel
        {
            Id = booking.BookingId,
            Time = new DateTimeOffset(booking.Time).ToUniversalTime().ToUnixTimeMilliseconds(),
            NrOfPersons = booking.NrOfPersons
        };

        return bookingViewModel;
    }
    
    public static BookingUserViewModel ToBookingUserViewModel(this Booking booking)
    {
        var bookingUserViewModel = new BookingUserViewModel
        {
            Time = new DateTimeOffset(booking.Time).ToUniversalTime().ToUnixTimeMilliseconds(),
            NrOfPersons = booking.NrOfPersons,
            FirstName = booking.User.FirstName,
            LastName = booking.User.LastName,
            Phone = booking.User.Phone
        };

        return bookingUserViewModel;
    }
}