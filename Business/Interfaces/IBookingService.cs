using Model.Entities;
using Model.ViewModels;

namespace Business.Interfaces;

public interface IBookingService
{
    Task<Booking?> AddBooking(Booking booking, int userId);
    List<int> GetAvailableNrOfPersons(long date);
    List<int> GetAvailableHours(long date, int nrOfPersons);
    Task<List<Booking>?> GetBookingsForUser(int userId);
    Task<bool> DeleteBooking(long bookingId);
    IAsyncEnumerable<BookingUserViewModel> GetAllBookingsByDate(long date);
}