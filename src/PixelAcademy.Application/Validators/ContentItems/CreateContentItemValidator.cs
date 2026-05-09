using FluentValidation;
using PixelAcademy.Application.Commands.ContentItems;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Validators.ContentItems;

public class CreateContentItemValidator : AbstractValidator<CreateContentItemCommand>
{
    public CreateContentItemValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description));
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DurationSeconds).GreaterThan(0)
            .When(x => x.Type == ContentItemType.Video && x.DurationSeconds.HasValue);
    }
}
