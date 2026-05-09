using System;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
    DateTime GetAccessTokenExpiration();
}
