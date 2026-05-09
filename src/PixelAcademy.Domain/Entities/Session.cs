using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Session : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public SessionType Type { get; set; } = SessionType.Web;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}
