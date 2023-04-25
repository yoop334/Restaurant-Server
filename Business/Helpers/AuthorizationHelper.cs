

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Account;
using Model.Entities;

namespace Business.Helpers;

public class AuthorizationHelper : IAuthorizationHelper
{
    private readonly AuthorizationSettings _authorizationSettings;
    private readonly IUserService _userService;

    public AuthorizationHelper(
        IOptions<AuthorizationSettings> authorizationSettings,
        IUserService userService)
    {
        _authorizationSettings = authorizationSettings.Value;
        _userService = userService;
    }

    public string? GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authorizationSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim("username", user.Username)
        };

        var token = new JwtSecurityToken(
            _authorizationSettings.Issuer,
            _authorizationSettings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool IsAccessTokenValid(string token)
    {
        SecurityToken validatedToken;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes(_authorizationSettings.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = _authorizationSettings.Issuer,
                    ValidAudience = _authorizationSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                }
                , out validatedToken);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public int ExtractUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

        return int.Parse(jwtToken.Subject);
    }

    public int? GetSubFromExpiredToken(string token)
    {
        var secretKey = Encoding.ASCII.GetBytes(_authorizationSettings.Secret);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _authorizationSettings.Issuer,
            ValidAudience = _authorizationSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
        return int.Parse(jwtToken.Subject);
    }
}