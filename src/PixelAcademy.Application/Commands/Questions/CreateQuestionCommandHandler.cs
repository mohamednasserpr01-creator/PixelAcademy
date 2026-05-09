using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Questions;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QuestionDto> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.ExamId);

        var question = new Question
        {
            Id = Guid.NewGuid(),
            ExamId = request.ExamId,
            Text = request.Text,
            Explanation = request.Explanation,
            Type = request.Type,
            OrderIndex = request.OrderIndex,
            Points = request.Points,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var opt in request.Options)
        {
            question.Options.Add(new QuestionOption
            {
                Id = Guid.NewGuid(),
                Text = opt.Text,
                OrderIndex = opt.OrderIndex,
                IsCorrect = opt.IsCorrect,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.Questions.AddAsync(question, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<QuestionDto>(question);
    }
}
