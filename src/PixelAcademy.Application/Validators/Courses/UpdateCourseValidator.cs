using FluentValidation;
using PixelAcademy.Application.Commands.Courses;

namespace PixelAcademy.Application.Validators.Courses;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Title));
        RuleFor(x => x.Description).MaximumLength(4000).When(x => !string.IsNullOrEmpty(x.Description));
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
    }
}
