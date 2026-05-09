using System;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.Exams;

public record CreateExamCommand(
    string Title,
    string? Description,
    Domain.Enums.ExamType Type,
    Guid? CourseId,
    Guid? LectureId,
    int? DurationMinutes,
    int AttemptLimit,
    int PassScorePercent,
    bool IsRandomized,
    DateTime? StartDate,
    DateTime? EndDate,
    bool ShowCorrectAnswers,
    Guid CreatedById
) : IRequest<ExamDto>;
