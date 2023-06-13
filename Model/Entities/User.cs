using Model.Enums;

namespace Model.Entities;

public class User
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public List<Booking> Bookings { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}