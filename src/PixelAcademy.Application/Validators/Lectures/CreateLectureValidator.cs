using FluentValidation;
using PixelAcademy.Application.Commands.Lectures;

namespace PixelAcademy.Application.Validators.Lectures;

public class CreateLectureValidator : AbstractValidator<CreateLectureCommand>
{
    public CreateLectureValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(0);
    }
}
