using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.Questions;

public record UpdateQuestionCommand(
    Guid Id,
    string Text,
    string? Explanation,
    int OrderIndex,
    int Points,
    List<CreateQuestionOptionRequest> Options
) : IRequest<QuestionDto>;
