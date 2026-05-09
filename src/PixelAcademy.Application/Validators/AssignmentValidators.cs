using FluentValidation;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Validators;

public class CreateAssignmentRequestDtoValidator : AbstractValidator<CreateAssignmentRequestDto>
{
    public CreateAssignmentRequestDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxPoints).GreaterThan(0).LessThanOrEqualTo(1000);
    }
}

public class UpdateAssignmentRequestDtoValidator : AbstractValidator<UpdateAssignmentRequestDto>
{
    public UpdateAssignmentRequestDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxPoints).GreaterThan(0).LessThanOrEqualTo(1000);
    }
}

public class SubmitAssignmentRequestDtoValidator : AbstractValidator<SubmitAssignmentRequestDto>
{
    public SubmitAssignmentRequestDtoValidator()
    {
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.TextAnswer) || !string.IsNullOrEmpty(x.FileUrl))
            .WithMessage("Either text answer or file URL must be provided.");
    }
}

public class GradeAssignmentRequestDtoValidator : AbstractValidator<GradeAssignmentRequestDto>
{
    public GradeAssignmentRequestDtoValidator()
    {
        RuleFor(x => x.SubmissionId).NotEmpty();
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
    }
}
