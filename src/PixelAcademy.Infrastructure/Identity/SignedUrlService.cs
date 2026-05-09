using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Infrastructure.Identity;

public class SignedUrlService : ISignedUrlService
{
    private readonly IConfiguration _configuration;

    public SignedUrlService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateSignedVideoUrl(Guid lectureId, Guid studentId, TimeSpan expiration)
    {
        var secret = _configuration["Jwt:Key"]!;
        var expiresAt = DateTimeOffset.UtcNow.Add(expiration).ToUnixTimeSeconds();
        var payload = $"{lectureId}:{studentId}:{expiresAt}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

        return $"/api/video/stream?t={token}&s={signature.Replace("+", "-").Replace("/", "_")}";
    }

    public bool ValidateSignedToken(string token, out Guid lectureId, out Guid studentId)
    {
        lectureId = Guid.Empty;
        studentId = Guid.Empty;

        try
        {
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = payload.Split(':');
            if (parts.Length != 3) return false;

            lectureId = Guid.Parse(parts[0]);
            studentId = Guid.Parse(parts[1]);
            var expiresAt = long.Parse(parts[2]);

            if (expiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}
