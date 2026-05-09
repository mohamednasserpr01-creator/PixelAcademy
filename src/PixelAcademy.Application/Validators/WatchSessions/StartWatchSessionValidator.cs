using FluentValidation;
using PixelAcademy.Application.Commands.WatchSessions;

namespace PixelAcademy.Application.Validators.WatchSessions;

public class StartWatchSessionValidator : AbstractValidator<StartWatchSessionCommand>
{
    public StartWatchSessionValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
