using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Questions;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, QuestionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QuestionDto> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _unitOfWork.Questions.GetWithOptionsAsync(request.Id, cancellationToken);
        if (question == null) throw new NotFoundException("Question", request.Id);

        question.Text = request.Text;
        question.Explanation = request.Explanation;
        question.OrderIndex = request.OrderIndex;
        question.Points = request.Points;

        // Replace options - clearing collection marks old options for deletion by EF cascade
        question.Options.Clear();

        foreach (var opt in request.Options)
        {
            question.Options.Add(new QuestionOption
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                Text = opt.Text,
                OrderIndex = opt.OrderIndex,
                IsCorrect = opt.IsCorrect,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<QuestionDto>(question);
    }
}
