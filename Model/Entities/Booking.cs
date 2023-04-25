namespace Model.Entities;

public class Booking
{
    public long BookingId { get; set; }
    public DateTime Time { get; set; }
    public int NrOfPersons { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
}