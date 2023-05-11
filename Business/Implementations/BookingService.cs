using Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Repository;

namespace Business.Implementations;

public class BookingService : IBookingService
{
    private const int NrOfTables = 10;
    private const int NrOfTablesForTwo = 5;
    private const int NrOfTablesForFour = 5;
    private const int HoursPerBooking = 2;
    private readonly DataContext _context;

    private readonly List<int> hours = new() { 10, 12, 14, 16, 18, 20 };

    private Dictionary<int, int> tablesMap = new() { { 2, NrOfTablesForTwo }, { 4, NrOfTablesForFour } };

    public BookingService(DataContext context)
    {
        _context = context;
    }

    public async Task<Booking?> AddBooking(Booking booking, int userId)
    {
        var user = _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
        if (user.Result == null)
        {
            return null;
        }

        booking.UserId = userId;
        booking.User = user.Result;
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>?> GetBookingsForUser(int userId)
    {
        var userWithBookings = await _context.Users.Include(user => user.Bookings)
            .FirstOrDefaultAsync(user => user.UserId == userId);

        if (userWithBookings != null)
        {
            return userWithBookings.Bookings;
        }
        return null;
    }

    public async Task<bool> DeleteBooking(long bookingId)
    {
        var existingBooking = await _context.Bookings.FirstOrDefaultAsync(booking => booking.BookingId == bookingId);
        if (existingBooking == null)
        {
            return false;
        }

        _context.Bookings.Remove(existingBooking);
        await _context.SaveChangesAsync();

        return true;
    }

    public List<int> GetAvailableNrOfPersons(long date)
    {
        var nrOfPersons = new List<int>();
        var selectedDate = DateTimeOffset.FromUnixTimeMilliseconds(date).LocalDateTime;

        var bookedTablesForTwo = _context.Bookings.Count(selectBooking =>
            selectBooking.Time.Date == selectedDate.Date && selectBooking.NrOfPersons == 2);
        if (NrOfTablesForTwo * 6 > bookedTablesForTwo) nrOfPersons.Add(2);

        var bookedTablesForFour = _context.Bookings.Count(selectBooking =>
            selectBooking.Time.Date == selectedDate.Date && selectBooking.NrOfPersons == 2);
        if (NrOfTablesForFour * 6 > bookedTablesForFour) nrOfPersons.Add(4);

        return nrOfPersons;
    }

    public List<int> GetAvailableHours(long date, int nrOfPersons)
    {
        var availableHours = new List<int>();
        
        var selectedDate = DateTimeOffset.FromUnixTimeMilliseconds(date).LocalDateTime;

        var bookedTables = _context.Bookings.Where(selectedBooking =>
            selectedBooking.NrOfPersons == nrOfPersons && selectedBooking.Time.Date == selectedDate.Date);

        var tablesForNrOfPersons = tablesMap[nrOfPersons];
        
        foreach (var hour in hours)
        {
            var bookedTablesByHour = bookedTables.Count(bookedTable => bookedTable.Time.Hour == hour);

            if (tablesForNrOfPersons > bookedTablesByHour)
            {
                availableHours.Add(hour);
            }
        }

        return availableHours;
    }
}