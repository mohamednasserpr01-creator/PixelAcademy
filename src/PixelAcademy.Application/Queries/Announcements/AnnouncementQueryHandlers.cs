using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Announcements;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Queries.Announcements;

public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, PagedResult<AnnouncementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAnnouncementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<AnnouncementDto>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _unitOfWork.Announcements.GetPublishedAsync(request.Page, request.PageSize, cancellationToken);
        var total = (await _unitOfWork.Announcements.GetAllAsync(cancellationToken)).Count(a => a.IsPublished && !a.IsDeleted);

        return new PagedResult<AnnouncementDto>
        {
            Items = _mapper.Map<IReadOnlyList<AnnouncementDto>>(announcements),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}

public class GetAnnouncementByIdQueryHandler : IRequestHandler<GetAnnouncementByIdQuery, AnnouncementDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAnnouncementByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AnnouncementDto?> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcement = await _unitOfWork.Announcements.GetByIdAsync(request.Id, cancellationToken);
        if (announcement == null || !announcement.IsPublished) return null;
        return _mapper.Map<AnnouncementDto>(announcement);
    }
}

public class GetCourseAnnouncementsQueryHandler : IRequestHandler<GetCourseAnnouncementsQuery, IReadOnlyList<AnnouncementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseAnnouncementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AnnouncementDto>> Handle(GetCourseAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await _unitOfWork.Announcements.GetByCourseAsync(request.CourseId, cancellationToken);
        return _mapper.Map<IReadOnlyList<AnnouncementDto>>(announcements);
    }
}
