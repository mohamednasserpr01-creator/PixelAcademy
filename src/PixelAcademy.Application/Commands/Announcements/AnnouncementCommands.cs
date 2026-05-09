using MediatR;
using PixelAcademy.Application.DTOs.Announcements;

namespace PixelAcademy.Application.Commands.Announcements;

public record CreateAnnouncementCommand(
    string Title,
    string Content,
    Domain.Enums.AnnouncementTarget Target,
    Guid? CourseId,
    DateTime? ScheduledPublishAt,
    int Priority,
    Guid CreatedById
) : IRequest<AnnouncementDto>;

public record UpdateAnnouncementCommand(
    Guid Id,
    string Title,
    string Content,
    DateTime? ScheduledPublishAt,
    int Priority,
    Guid UpdatedById
) : IRequest<AnnouncementDto>;

public record DeleteAnnouncementCommand(Guid Id, Guid DeletedById) : IRequest<bool>;

public record PublishAnnouncementCommand(Guid Id, Guid PublishedById) : IRequest<AnnouncementDto>;
