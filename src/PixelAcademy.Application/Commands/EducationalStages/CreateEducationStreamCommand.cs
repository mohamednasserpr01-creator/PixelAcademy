using MediatR;
using System;

namespace PixelAcademy.Application.Commands.EducationalStages;

public class CreateEducationStreamCommand : IRequest<Guid>
{
    public Guid EducationalStageId { get; set; }
    public string Name { get; set; }
}