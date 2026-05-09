using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class AuditLog : AuditableEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public AuditActionType Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Details { get; set; }
    public bool IsAdminAction { get; set; }
}
