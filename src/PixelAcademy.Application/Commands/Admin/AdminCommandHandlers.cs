using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Application.Commands.Admin;

public class BanUserCommandHandler : IRequestHandler<BanUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BanUserCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException("User", request.UserId);

        user.IsBanned = true;
        user.BannedAt = _dateTimeProvider.UtcNow;
        user.BannedReason = request.Reason;
        user.IsActive = false;

        await LogAuditAsync(request.AdminId, AuditActionType.BanUser, "User", request.UserId, $"Banned user: {request.Reason}", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}

public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnbanUserCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException("User", request.UserId);

        user.IsBanned = false;
        user.BannedAt = null;
        user.BannedReason = null;
        user.IsActive = true;

        await LogAuditAsync(request.AdminId, AuditActionType.UnbanUser, "User", request.UserId, "Unbanned user", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}

public class DisableCourseCommandHandler : IRequestHandler<DisableCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DisableCourseCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(DisableCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        course.IsDisabled = true;

        await LogAuditAsync(request.AdminId, AuditActionType.DisableCourse, "Course", request.CourseId, "Disabled course", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}

public class EnableCourseCommandHandler : IRequestHandler<EnableCourseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EnableCourseCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(EnableCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        course.IsDisabled = false;

        await LogAuditAsync(request.AdminId, AuditActionType.EnableCourse, "Course", request.CourseId, "Enabled course", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}

public class DisableLectureCommandHandler : IRequestHandler<DisableLectureCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DisableLectureCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(DisableLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        lecture.IsDisabled = true;

        await LogAuditAsync(request.AdminId, AuditActionType.DisableLecture, "Lecture", request.LectureId, "Disabled lecture", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}

public class EnableLectureCommandHandler : IRequestHandler<EnableLectureCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EnableLectureCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(EnableLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        lecture.IsDisabled = false;

        await LogAuditAsync(request.AdminId, AuditActionType.EnableLecture, "Lecture", request.LectureId, "Enabled lecture", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task LogAuditAsync(Guid? userId, AuditActionType action, string entityType, Guid entityId, string details, CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IsAdminAction = true,
            CreatedAt = _dateTimeProvider.UtcNow
        };
        await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
    }
}
