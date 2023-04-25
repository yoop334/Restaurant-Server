using Model.ViewModels.User;

namespace Model.Account;

public class AuthenticationResponse
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public UserViewModel? User { get; set; }
}