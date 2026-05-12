using MediatR;
using System;

namespace PixelAcademy.Application.Commands.Subjects;

public class CreateSubjectCommand : IRequest<Guid>
{
    public string Name { get; set; }
}