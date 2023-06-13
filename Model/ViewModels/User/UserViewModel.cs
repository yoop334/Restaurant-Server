using Model.Enums;

namespace Model.ViewModels.User;

public class UserViewModel
{
    public long Id { get; set; }

    public string Username { get; set; }
    
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public string Phone { get; set; }
    
    public UserRole Role { get; set; }
}