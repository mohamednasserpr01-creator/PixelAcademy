using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.ExamAttempts;

public class SubmitExamAttemptCommandHandler : IRequestHandler<SubmitExamAttemptCommand, ExamResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SubmitExamAttemptCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ExamResultDto> Handle(SubmitExamAttemptCommand request, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.ExamAttempts.GetByIdAsync(request.ExamAttemptId, cancellationToken);
        if (attempt == null) throw new NotFoundException("Exam attempt", request.ExamAttemptId);
        if (attempt.StudentId != request.StudentId)
            throw new ForbiddenException("You can only submit your own attempts.");
        if (attempt.Status != ExamAttemptStatus.InProgress)
            throw new BadRequestException("This attempt has already been submitted or timed out.");

        var exam = await _unitOfWork.Exams.GetWithQuestionsAsync(attempt.ExamId, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", attempt.ExamId);

        // Check timed expiration
        if (exam.DurationMinutes.HasValue)
        {
            var elapsed = (int)(_dateTimeProvider.UtcNow - attempt.StartedAt).TotalSeconds;
            var allowedSeconds = exam.DurationMinutes.Value * 60;
            if (elapsed > allowedSeconds + 30) // 30-second grace
            {
                attempt.Status = ExamAttemptStatus.TimedOut;
                attempt.SubmittedAt = _dateTimeProvider.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new BadRequestException("Your time has expired for this exam.");
            }
        }

        int score = 0;
        int totalPoints = exam.Questions.Sum(q => q.Points);

        foreach (var answerReq in request.Answers)
        {
            var question = exam.Questions.FirstOrDefault(q => q.Id == answerReq.QuestionId);
            if (question == null) continue;

            var answer = attempt.Answers.FirstOrDefault(a => a.QuestionId == answerReq.QuestionId);
            if (answer == null)
            {
                answer = new ExamAnswer
                {
                    Id = Guid.NewGuid(),
                    ExamAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    CreatedAt = _dateTimeProvider.UtcNow
                };
                await _unitOfWork.ExamAnswers.AddAsync(answer, cancellationToken);
                attempt.Answers.Add(answer);
            }

            answer.SelectedOptionIds = answerReq.SelectedOptionId ?? (answerReq.SelectedOptionIds != null ? string.Join(",", answerReq.SelectedOptionIds) : null);
            answer.TextAnswer = answerReq.TextAnswer;

            // Auto-grade objective questions
            if (question.Type == QuestionType.MultipleChoice || question.Type == QuestionType.TrueFalse)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null && answerReq.SelectedOptionId != null
                    && Guid.TryParse(answerReq.SelectedOptionId, out var selectedGuid)
                    && selectedGuid == correctOption.Id)
                {
                    answer.IsCorrect = true;
                    answer.PointsEarned = question.Points;
                    score += question.Points;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.PointsEarned = 0;
                }
            }
            else if (question.Type == QuestionType.MultiSelect)
            {
                var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();
                var selectedIds = answerReq.SelectedOptionIds?.Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToHashSet() ?? new HashSet<Guid>();
                if (selectedIds.SetEquals(correctOptionIds))
                {
                    answer.IsCorrect = true;
                    answer.PointsEarned = question.Points;
                    score += question.Points;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.PointsEarned = 0;
                }
            }
            else
            {
                // Short answer - manual grading
                answer.IsCorrect = null;
                answer.PointsEarned = null;
            }
        }

        attempt.Score = score;
        attempt.TotalPoints = totalPoints;
        attempt.IsPassed = totalPoints > 0 ? (score * 100 / totalPoints) >= exam.PassScorePercent : false;
        attempt.Status = ExamAttemptStatus.Submitted;
        attempt.SubmittedAt = _dateTimeProvider.UtcNow;
        attempt.DurationSeconds = (int)(_dateTimeProvider.UtcNow - attempt.StartedAt).TotalSeconds;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var percentage = totalPoints > 0 ? (double)score / totalPoints * 100 : 0;
        return new ExamResultDto
        {
            AttemptId = attempt.Id,
            ExamTitle = exam.Title,
            Score = score,
            TotalPoints = totalPoints,
            Percentage = Math.Round(percentage, 2),
            IsPassed = attempt.IsPassed ?? false,
            PassScorePercent = exam.PassScorePercent,
            DurationSeconds = attempt.DurationSeconds ?? 0,
            StartedAt = attempt.StartedAt,
            SubmittedAt = attempt.SubmittedAt,
            ShowCorrectAnswers = exam.ShowCorrectAnswers,
            AnswerResults = BuildAnswerResults(attempt, exam, exam.ShowCorrectAnswers)
        };
    }

    private List<ExamAnswerResultDto> BuildAnswerResults(ExamAttempt attempt, Exam exam, bool showCorrect)
    {
        var results = new List<ExamAnswerResultDto>();
        foreach (var answer in attempt.Answers)
        {
            var question = exam.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question == null) continue;

            var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id.ToString()).ToList();
            var correctText = question.Type == QuestionType.ShortAnswer
                ? null
                : string.Join(", ", question.Options.Where(o => o.IsCorrect).Select(o => o.Text));

            var options = _mapper.Map<List<QuestionOptionDetailDto>>(question.Options);
            if (!showCorrect)
            {
                foreach (var opt in options) opt.IsCorrect = false;
            }

            results.Add(new ExamAnswerResultDto
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                QuestionType = question.Type,
                Points = question.Points,
                PointsEarned = answer.PointsEarned ?? 0,
                IsCorrect = answer.IsCorrect ?? false,
                StudentAnswer = answer.TextAnswer ?? answer.SelectedOptionIds,
                CorrectAnswer = showCorrect ? correctText : null,
                Explanation = showCorrect ? question.Explanation : null,
                Options = options
            });
        }
        return results;
    }
}
