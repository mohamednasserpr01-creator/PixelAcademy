using FluentValidation;
using PixelAcademy.Application.DTOs.Exams;

namespace PixelAcademy.Application.Validators;

public class CreateExamRequestDtoValidator : AbstractValidator<CreateExamRequestDto>
{
    public CreateExamRequestDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AttemptLimit).GreaterThan(0).LessThanOrEqualTo(10);
        RuleFor(x => x.PassScorePercent).InclusiveBetween(0, 100);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).When(x => x.DurationMinutes.HasValue);
    }
}

public class UpdateExamRequestDtoValidator : AbstractValidator<UpdateExamRequestDto>
{
    public UpdateExamRequestDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AttemptLimit).GreaterThan(0).LessThanOrEqualTo(10);
        RuleFor(x => x.PassScorePercent).InclusiveBetween(0, 100);
    }
}

public class CreateQuestionRequestDtoValidator : AbstractValidator<CreateQuestionRequestDto>
{
    public CreateQuestionRequestDtoValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Points).GreaterThan(0);
        RuleFor(x => x.Options).NotEmpty().When(x => x.Type != Domain.Enums.QuestionType.ShortAnswer);
        RuleForEach(x => x.Options).ChildRules(opt =>
        {
            opt.RuleFor(x => x.Text).NotEmpty();
        });
    }
}

public class SubmitExamAttemptRequestDtoValidator : AbstractValidator<SubmitExamAttemptRequestDto>
{
    public SubmitExamAttemptRequestDtoValidator()
    {
        RuleFor(x => x.ExamAttemptId).NotEmpty();
        RuleFor(x => x.Answers).NotNull();
    }
}
