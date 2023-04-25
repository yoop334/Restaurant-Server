﻿namespace Model.Account;

public class AuthorizationSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}