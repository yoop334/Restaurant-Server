﻿using Model.Entities;

namespace Business.Interfaces;

public interface IAuthorizationHelper
{
    bool IsAccessTokenValid(string token);
    Task<bool> IsUsersRoleAuthorized(string token, string authorizedRoles);
    int ExtractUserIdFromToken(string token);
    string? GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int? GetSubFromExpiredToken(string token);
}