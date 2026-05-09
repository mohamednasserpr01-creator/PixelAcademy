using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Media;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Media;

public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaAssetDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadMediaCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<MediaAssetDto> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        if (request.CourseId.HasValue)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId.Value, cancellationToken);
            if (course == null) throw new NotFoundException("Course", request.CourseId.Value);
        }

        if (request.LectureId.HasValue)
        {
            var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId.Value, cancellationToken);
            if (lecture == null) throw new NotFoundException("Lecture", request.LectureId.Value);
        }

        var media = new MediaAsset
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            OriginalFileName = request.OriginalFileName,
            Url = request.Url,
            Type = request.Type,
            FileSize = request.FileSize,
            MimeType = request.MimeType,
            DurationSeconds = request.DurationSeconds,
            Width = request.Width,
            Height = request.Height,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            UploadedById = request.UploadedById,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.MediaAssets.AddAsync(media, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<MediaAssetDto>(media);
    }
}
