using MediatR;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Application.Commands.EducationalStages;

public class GetEducationalStagesQuery : IRequest<List<EducationalStage>>
{
}