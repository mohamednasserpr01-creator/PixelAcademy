using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Queries.Exams;

public class GetExamsQueryHandler : IRequestHandler<GetExamsQuery, IReadOnlyList<ExamDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExamsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ExamDto>> Handle(GetExamsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Exam> exams;
        if (request.CourseId.HasValue)
            exams = await _unitOfWork.Exams.GetByCourseAsync(request.CourseId.Value, request.IncludeUnpublished, cancellationToken);
        else
        {
            var all = await _unitOfWork.Exams.GetAllAsync(cancellationToken);
            exams = request.IncludeUnpublished ? all : all.Where(e => e.IsPublished).ToList();
        }
        return _mapper.Map<IReadOnlyList<ExamDto>>(exams);
    }
}

public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, ExamDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExamByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExamDetailDto?> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetWithQuestionsAsync(request.Id, cancellationToken);
        if (exam == null) return null;
        return _mapper.Map<ExamDetailDto>(exam);
    }
}

public class GetExamQuestionsQueryHandler : IRequestHandler<GetExamQuestionsQuery, IReadOnlyList<QuestionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExamQuestionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<QuestionDto>> Handle(GetExamQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _unitOfWork.Questions.GetByExamAsync(request.ExamId, cancellationToken);
        return _mapper.Map<IReadOnlyList<QuestionDto>>(questions);
    }
}

public class GetExamAttemptQueryHandler : IRequestHandler<GetExamAttemptQuery, ExamAttemptDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetExamAttemptQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ExamAttemptDto?> Handle(GetExamAttemptQuery request, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.ExamAttempts.GetWithAnswersAsync(request.AttemptId, cancellationToken);
        if (attempt == null) return null;
        return _mapper.Map<ExamAttemptDto>(attempt);
    }
}

public class GetStudentExamResultsQueryHandler : IRequestHandler<GetStudentExamResultsQuery, IReadOnlyList<ExamResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentExamResultsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ExamResultDto>> Handle(GetStudentExamResultsQuery request, CancellationToken cancellationToken)
    {
        var attempts = await _unitOfWork.ExamAttempts.GetAllAsync(cancellationToken);
        var filtered = attempts
            .Where(a => a.StudentId == request.StudentId && a.Status == Domain.Enums.ExamAttemptStatus.Submitted)
            .ToList();

        if (request.ExamId.HasValue)
            filtered = filtered.Where(a => a.ExamId == request.ExamId.Value).ToList();

        var results = new List<ExamResultDto>();
        foreach (var attempt in filtered)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(attempt.ExamId, cancellationToken);
            if (exam == null) continue;
            var percentage = attempt.TotalPoints > 0 ? (double)(attempt.Score ?? 0) / attempt.TotalPoints.Value * 100 : 0;
            results.Add(new ExamResultDto
            {
                AttemptId = attempt.Id,
                ExamTitle = exam.Title,
                Score = attempt.Score ?? 0,
                TotalPoints = attempt.TotalPoints ?? 0,
                Percentage = Math.Round(percentage, 2),
                IsPassed = attempt.IsPassed ?? false,
                PassScorePercent = exam.PassScorePercent,
                DurationSeconds = attempt.DurationSeconds ?? 0,
                StartedAt = attempt.StartedAt,
                SubmittedAt = attempt.SubmittedAt,
                ShowCorrectAnswers = exam.ShowCorrectAnswers
            });
        }
        return results;
    }
}

public class GetExamAnalyticsQueryHandler : IRequestHandler<GetExamAnalyticsQuery, ExamAnalyticsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExamAnalyticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ExamAnalyticsDto> Handle(GetExamAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetWithQuestionsAsync(request.ExamId, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.ExamId);

        var attempts = await _unitOfWork.ExamAttempts.GetByExamAsync(request.ExamId, cancellationToken);
        var completed = attempts.Where(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted).ToList();

        var scores = completed.Select(a => a.Score ?? 0).ToList();
        var avgScore = completed.Any() ? scores.Average() : 0;
        var avgPercentage = completed.Any() && exam.Questions.Any()
            ? completed.Average(a => a.TotalPoints > 0 ? (double)(a.Score ?? 0) / a.TotalPoints.Value * 100 : 0)
            : 0;
        var passRate = completed.Any()
            ? (double)completed.Count(a => a.IsPassed == true) / completed.Count * 100
            : 0;

        var questionAnalytics = new List<QuestionAnalyticsDto>();
        foreach (var question in exam.Questions)
        {
            var answers = await _unitOfWork.ExamAnswers.GetByAttemptAsync(
                attempts.Where(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted).Select(a => a.Id).FirstOrDefault(), cancellationToken);
            // Actually we need all answers for this question across all attempts
            var allAnswers = (await _unitOfWork.ExamAnswers.GetAllAsync(cancellationToken))
                .Where(a => a.QuestionId == question.Id && completed.Select(x => x.Id).Contains(a.ExamAttemptId))
                .ToList();

            var total = allAnswers.Count;
            var correct = allAnswers.Count(a => a.IsCorrect == true);
            var rate = total > 0 ? (double)correct / total * 100 : 0;
            questionAnalytics.Add(new QuestionAnalyticsDto
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                TotalAnswers = total,
                CorrectAnswers = correct,
                CorrectRate = Math.Round(rate, 2),
                IsHardest = false
            });
        }

        if (questionAnalytics.Any())
        {
            var minRate = questionAnalytics.Min(q => q.CorrectRate);
            foreach (var q in questionAnalytics.Where(q => q.CorrectRate == minRate))
                q.IsHardest = true;
        }

        return new ExamAnalyticsDto
        {
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            TotalAttempts = attempts.Count,
            CompletedAttempts = completed.Count,
            AverageScore = Math.Round(avgScore, 2),
            AveragePercentage = Math.Round(avgPercentage, 2),
            PassRate = Math.Round(passRate, 2),
            HighestScore = scores.Any() ? scores.Max() : 0,
            LowestScore = scores.Any() ? scores.Min() : 0,
            QuestionAnalytics = questionAnalytics
        };
    }
}
