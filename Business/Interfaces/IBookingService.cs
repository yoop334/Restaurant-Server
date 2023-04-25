using Model.Entities;
using Model.ViewModels.Booking;

namespace Business.Interfaces;

public interface IBookingService
{
    Task<Booking?> AddBooking(Booking booking, int userId);
    List<int> GetAvailableNrOfPersons(long date);
    List<int> GetAvailableHours(long date, int nrOfPersons);
    Task<List<Booking>?> GetBookingsForUser(int userId);
}