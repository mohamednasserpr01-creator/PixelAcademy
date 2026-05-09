using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Exams;

public class PublishExamCommandHandler : IRequestHandler<PublishExamCommand, ExamDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PublishExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExamDto> Handle(PublishExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(request.Id, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.Id);
        if (exam.CreatedById != request.UpdatedById)
            throw new ForbiddenException("You can only publish your own exams.");

        exam.IsPublished = request.IsPublished;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ExamDto>(exam);
    }
}
