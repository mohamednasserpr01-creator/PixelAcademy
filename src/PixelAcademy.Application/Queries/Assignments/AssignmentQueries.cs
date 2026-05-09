using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Queries.Assignments;

public record GetAssignmentsQuery(Guid? CourseId) : IRequest<IReadOnlyList<AssignmentDto>>;
public record GetAssignmentByIdQuery(Guid Id) : IRequest<AssignmentDto?>;
public record GetAssignmentSubmissionsQuery(Guid AssignmentId) : IRequest<IReadOnlyList<AssignmentSubmissionDto>>;
public record GetMySubmissionQuery(Guid StudentId, Guid AssignmentId) : IRequest<AssignmentSubmissionDto?>;
