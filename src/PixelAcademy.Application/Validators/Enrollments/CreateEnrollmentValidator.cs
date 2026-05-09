using FluentValidation;
using PixelAcademy.Application.Commands.Enrollments;

namespace PixelAcademy.Application.Validators.Enrollments;

public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
