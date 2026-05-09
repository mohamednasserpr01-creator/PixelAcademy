using System;
using System.Collections.Generic;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Queries.Exams;

public record GetExamsQuery(Guid? CourseId, bool IncludeUnpublished = false) : IRequest<IReadOnlyList<ExamDto>>;
public record GetExamByIdQuery(Guid Id) : IRequest<ExamDetailDto?>;
public record GetExamQuestionsQuery(Guid ExamId) : IRequest<IReadOnlyList<QuestionDto>>;
public record GetExamAttemptQuery(Guid AttemptId) : IRequest<ExamAttemptDto?>;
public record GetStudentExamResultsQuery(Guid StudentId, Guid? ExamId = null) : IRequest<IReadOnlyList<ExamResultDto>>;
public record GetExamAnalyticsQuery(Guid ExamId) : IRequest<ExamAnalyticsDto>;
