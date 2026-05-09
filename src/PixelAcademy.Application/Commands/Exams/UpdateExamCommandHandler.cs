using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Exams;

public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, ExamDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExamDto> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(request.Id, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.Id);
        if (exam.CreatedById != request.UpdatedById)
            throw new ForbiddenException("You can only edit your own exams.");

        exam.Title = request.Title;
        exam.Description = request.Description;
        exam.DurationMinutes = request.DurationMinutes;
        exam.AttemptLimit = request.AttemptLimit;
        exam.PassScorePercent = request.PassScorePercent;
        exam.IsRandomized = request.IsRandomized;
        exam.StartDate = request.StartDate;
        exam.EndDate = request.EndDate;
        exam.ShowCorrectAnswers = request.ShowCorrectAnswers;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ExamDto>(exam);
    }
}
