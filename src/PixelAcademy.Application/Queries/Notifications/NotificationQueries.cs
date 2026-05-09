using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Notifications;

namespace PixelAcademy.Application.Queries.Notifications;

public record GetNotificationsQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<NotificationDto>>;
public record GetUnreadNotificationsQuery(Guid UserId) : IRequest<IReadOnlyList<NotificationDto>>;
public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<int>;
public record GetNotificationByIdQuery(Guid Id, Guid UserId) : IRequest<NotificationDto?>;
