using MediatR;
using PixelAcademy.Application.DTOs.Notifications;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.Notifications;

public record CreateNotificationCommand(
    Guid UserId,
    string Title,
    string Message,
    NotificationType Type,
    string? ActionUrl,
    Guid? RelatedEntityId,
    string? RelatedEntityType
) : IRequest<NotificationDto>;

public record MarkNotificationAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<bool>;

public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<int>;

public record DeleteNotificationCommand(Guid NotificationId, Guid UserId) : IRequest<bool>;
