using Model.Account;

namespace Business.Interfaces;

public interface IAccountService
{
    Task<AuthenticationResponse?> AuthenticateAsync(string username, string password);
    Task<RefreshTokenResponse?> RefreshAsync(RefreshTokenRequest refreshTokenModel);
    Task<bool> RevokeRefreshToken(int userId);
    Task<bool> CheckToken(int userId);
}