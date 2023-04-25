using System.ComponentModel.DataAnnotations;

namespace Model.Account;

public class AuthenticationRequest
{
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }
}