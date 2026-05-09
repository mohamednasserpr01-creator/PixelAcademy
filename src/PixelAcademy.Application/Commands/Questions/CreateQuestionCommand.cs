using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.Questions;

public record CreateQuestionCommand(
    Guid ExamId,
    string Text,
    string? Explanation,
    Domain.Enums.QuestionType Type,
    int OrderIndex,
    int Points,
    List<CreateQuestionOptionRequest> Options
) : IRequest<QuestionDto>;

public record CreateQuestionOptionRequest(
    string Text,
    int OrderIndex,
    bool IsCorrect
);
