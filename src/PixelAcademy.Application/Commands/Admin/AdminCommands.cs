using MediatR;

namespace PixelAcademy.Application.Commands.Admin;

public record BanUserCommand(Guid UserId, string Reason, Guid AdminId) : IRequest<bool>;
public record UnbanUserCommand(Guid UserId, Guid AdminId) : IRequest<bool>;
public record DisableCourseCommand(Guid CourseId, Guid AdminId) : IRequest<bool>;
public record EnableCourseCommand(Guid CourseId, Guid AdminId) : IRequest<bool>;
public record DisableLectureCommand(Guid LectureId, Guid AdminId) : IRequest<bool>;
public record EnableLectureCommand(Guid LectureId, Guid AdminId) : IRequest<bool>;
