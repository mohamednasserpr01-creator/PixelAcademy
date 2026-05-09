using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Commands.Exams;

public record PublishExamCommand(Guid Id, bool IsPublished, Guid UpdatedById) : IRequest<ExamDto>;
