using FluentValidation;
using PixelAcademy.Application.Commands.WatchSessions;

namespace PixelAcademy.Application.Validators.WatchSessions;

public class UpdateWatchProgressValidator : AbstractValidator<UpdateWatchProgressCommand>
{
    public UpdateWatchProgressValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CurrentPositionSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DurationWatchedSeconds).GreaterThanOrEqualTo(0);
    }
}
