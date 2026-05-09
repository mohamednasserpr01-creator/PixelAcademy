using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.ExamAttempts;

public record StartExamAttemptCommand(Guid ExamId, Guid StudentId) : IRequest<ExamAttemptDto>;
