﻿using Model.Enums;

namespace Model.Account;

public class RefreshTokenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    
    public UserRole Role { get; set; }
}