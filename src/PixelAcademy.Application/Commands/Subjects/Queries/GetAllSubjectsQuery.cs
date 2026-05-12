using MediatR;
using System;
using System.Collections.Generic;

namespace PixelAcademy.Application.Queries.Subjects;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class GetAllSubjectsQuery : IRequest<IEnumerable<SubjectDto>>
{
}