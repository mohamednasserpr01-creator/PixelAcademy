using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.Exams;

public record UpdateExamCommand(
    Guid Id,
    string Title,
    string? Description,
    int? DurationMinutes,
    int AttemptLimit,
    int PassScorePercent,
    bool IsRandomized,
    DateTime? StartDate,
    DateTime? EndDate,
    bool ShowCorrectAnswers,
    Guid UpdatedById
) : IRequest<ExamDto>;
