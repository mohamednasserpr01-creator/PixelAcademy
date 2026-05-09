using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Notification : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.General;
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
    public string? ActionUrl { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public DateTime? ReadAt { get; set; }
}
