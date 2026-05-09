using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class GenerateActivationCodeCommandHandler : IRequestHandler<GenerateActivationCodeCommand, ActivationCodeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GenerateActivationCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ActivationCodeDto> Handle(GenerateActivationCodeCommand request, CancellationToken cancellationToken)
    {
        var generator = await _unitOfWork.Users.GetByIdAsync(request.GeneratedById, cancellationToken);
        if (generator == null) throw new NotFoundException("User", request.GeneratedById);

        if (request.Type == CodeType.CourseEnrollment && request.CourseId.HasValue)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId.Value, cancellationToken);
            if (course == null) throw new NotFoundException("Course", request.CourseId.Value);
        }

        if (request.Type == CodeType.LectureAccess && request.LectureId.HasValue)
        {
            var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId.Value, cancellationToken);
            if (lecture == null) throw new NotFoundException("Lecture", request.LectureId.Value);
        }

        string code;
        do
        {
            code = $"PA-{Guid.NewGuid():N}".Substring(0, 16).ToUpperInvariant();
        } while (await _unitOfWork.ActivationCodes.CodeExistsAsync(code, cancellationToken));

        var activationCode = new ActivationCode
        {
            Id = Guid.NewGuid(),
            Code = code,
            Type = request.Type,
            Value = request.Value,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            MaxRedemptions = request.MaxRedemptions ?? 1,
            CurrentRedemptions = 0,
            ExpiresAt = request.ExpiresAt,
            IsActive = true,
            GeneratedById = request.GeneratedById,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.ActivationCodes.AddAsync(activationCode, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ActivationCodeDto>(activationCode);
    }
}
