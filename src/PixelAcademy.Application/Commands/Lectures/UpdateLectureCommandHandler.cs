using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Lectures;

public class UpdateLectureCommandHandler : IRequestHandler<UpdateLectureCommand, LectureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateLectureCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LectureDto> Handle(UpdateLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.Id, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.Id);

        if (!string.IsNullOrEmpty(request.Title)) lecture.Title = request.Title;
        if (request.Description != null) lecture.Description = request.Description;
        if (request.OrderIndex.HasValue) lecture.OrderIndex = request.OrderIndex.Value;
        if (request.DurationMinutes.HasValue) lecture.DurationMinutes = request.DurationMinutes.Value;
        if (request.IsPreview.HasValue) lecture.IsPreview = request.IsPreview.Value;
        if (request.VideoUrl != null) lecture.VideoUrl = request.VideoUrl;

        lecture.UpdatedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.Lectures.UpdateAsync(lecture, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LectureDto>(lecture);
    }
}
