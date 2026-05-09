using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class GenerateSignedVideoUrlCommandHandler : IRequestHandler<GenerateSignedVideoUrlCommand, SignedVideoUrlDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISignedUrlService _signedUrlService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateSignedVideoUrlCommandHandler(IUnitOfWork unitOfWork, ISignedUrlService signedUrlService, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _signedUrlService = signedUrlService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SignedVideoUrlDto> Handle(GenerateSignedVideoUrlCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        // Validate access
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, lecture.CourseId, cancellationToken);
        var hasLectureAccess = await _unitOfWork.LectureAccesses.HasAccessAsync(request.StudentId, request.LectureId, cancellationToken);

        if (!isEnrolled && !hasLectureAccess && !lecture.IsPreview)
            throw new ForbiddenException("You do not have access to this lecture.");

        var expiration = TimeSpan.FromMinutes(10);
        var signedUrl = _signedUrlService.GenerateSignedVideoUrl(request.LectureId, request.StudentId, expiration);

        return new SignedVideoUrlDto
        {
            SignedUrl = signedUrl,
            ExpiresAt = _dateTimeProvider.UtcNow.Add(expiration),
            LectureTitle = lecture.Title
        };
    }
}
