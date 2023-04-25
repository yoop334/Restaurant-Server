using Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Account;
using Model.Mappers;
using Repository;

namespace Business.Implementations;

public class AccountService : IAccountService
{
    private readonly IAuthorizationHelper _authorizationHelper;
    private readonly DataContext _context;
    private readonly IHashingHelper _hashingHelper;

    public AccountService(DataContext dataContext, IAuthorizationHelper authorizationHelper,
        IHashingHelper hashingHelper)
    {
        _context = dataContext;
        _hashingHelper = hashingHelper;
        _authorizationHelper = authorizationHelper;
    }

    public async Task<AuthenticationResponse?> AuthenticateAsync(string username, string password)
    {
        var loggingInUser = _context.Users
            .FirstOrDefault(user =>
                user.Username == username && user.Password == _hashingHelper.HashPassword(password)
            );

        if (loggingInUser == default) return null;

        var authenticationResponse = new AuthenticationResponse
        {
            AccessToken = _authorizationHelper.GenerateAccessToken(loggingInUser),
            RefreshToken = _authorizationHelper.GenerateRefreshToken(),
            User = loggingInUser.ToViewModel()
        };

        loggingInUser.RefreshToken = authenticationResponse.RefreshToken;
        loggingInUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);

        await _context.SaveChangesAsync();

        return authenticationResponse;
    }

    public async Task<RefreshTokenResponse?> RefreshAsync(RefreshTokenRequest refreshTokenModel)
    {
        var userId = _authorizationHelper.GetSubFromExpiredToken(refreshTokenModel.AccessToken);

        if (userId == null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
        if (user == null || user.RefreshToken != refreshTokenModel.RefreshToken) return null;

        var refreshTokenResponse = new RefreshTokenResponse
        {
            AccessToken = _authorizationHelper.GenerateAccessToken(user),
            RefreshToken = _authorizationHelper.GenerateRefreshToken()
        };

        user.RefreshToken = refreshTokenResponse.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
        await _context.SaveChangesAsync();

        return refreshTokenResponse;
    }

    public async Task<bool> RevokeRefreshToken(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
        if (string.IsNullOrEmpty(user?.RefreshToken)) return false;

        user.RefreshToken = null;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckToken(int userId)
    { 
        var user = await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);

        if (user == null) return false;

        var refreshTokenExpiryTime = user.RefreshTokenExpiryTime;

        return refreshTokenExpiryTime != null;
    }
}