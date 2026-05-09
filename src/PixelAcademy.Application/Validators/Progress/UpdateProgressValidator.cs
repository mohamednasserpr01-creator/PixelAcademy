using FluentValidation;
using PixelAcademy.Application.Commands.Progress;

namespace PixelAcademy.Application.Validators.Progress;

public class UpdateProgressValidator : AbstractValidator<UpdateVideoProgressCommand>
{
    public UpdateProgressValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.WatchedSeconds).GreaterThanOrEqualTo(0);
    }
}
