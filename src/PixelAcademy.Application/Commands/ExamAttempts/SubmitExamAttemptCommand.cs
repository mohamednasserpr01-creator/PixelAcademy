using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.ExamAttempts;

public record SubmitExamAttemptCommand(
    Guid ExamAttemptId,
    List<SubmitAnswerRequest> Answers,
    Guid StudentId
) : IRequest<ExamResultDto>;

public record SubmitAnswerRequest(
    Guid QuestionId,
    string? SelectedOptionId,
    List<string>? SelectedOptionIds,
    string? TextAnswer
);
