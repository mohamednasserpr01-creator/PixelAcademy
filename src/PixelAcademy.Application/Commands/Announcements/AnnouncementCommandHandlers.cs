using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Announcements;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Announcements;

public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, AnnouncementDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateAnnouncementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AnnouncementDto> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        if (request.CourseId.HasValue)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId.Value, cancellationToken);
            if (course == null) throw new NotFoundException("Course", request.CourseId.Value);
        }

        var announcement = new Announcement
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            Target = request.Target,
            CourseId = request.CourseId,
            CreatedById = request.CreatedById,
            ScheduledPublishAt = request.ScheduledPublishAt,
            Priority = request.Priority,
            IsPublished = false,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.Announcements.AddAsync(announcement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AnnouncementDto>(announcement);
    }
}

public class UpdateAnnouncementCommandHandler : IRequestHandler<UpdateAnnouncementCommand, AnnouncementDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAnnouncementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AnnouncementDto> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = await _unitOfWork.Announcements.GetByIdAsync(request.Id, cancellationToken);
        if (announcement == null) throw new NotFoundException("Announcement", request.Id);
        if (announcement.CreatedById != request.UpdatedById)
            throw new ForbiddenException("You can only edit your own announcements.");

        announcement.Title = request.Title;
        announcement.Content = request.Content;
        announcement.ScheduledPublishAt = request.ScheduledPublishAt;
        announcement.Priority = request.Priority;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AnnouncementDto>(announcement);
    }
}

public class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAnnouncementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = await _unitOfWork.Announcements.GetByIdAsync(request.Id, cancellationToken);
        if (announcement == null) throw new NotFoundException("Announcement", request.Id);
        if (announcement.CreatedById != request.DeletedById)
            throw new ForbiddenException("You can only delete your own announcements.");

        await _unitOfWork.Announcements.DeleteAsync(announcement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class PublishAnnouncementCommandHandler : IRequestHandler<PublishAnnouncementCommand, AnnouncementDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PublishAnnouncementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AnnouncementDto> Handle(PublishAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = await _unitOfWork.Announcements.GetByIdAsync(request.Id, cancellationToken);
        if (announcement == null) throw new NotFoundException("Announcement", request.Id);

        announcement.IsPublished = true;
        announcement.PublishedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AnnouncementDto>(announcement);
    }
}
