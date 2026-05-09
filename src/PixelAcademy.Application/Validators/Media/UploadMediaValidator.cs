using FluentValidation;
using PixelAcademy.Application.Commands.Media;

namespace PixelAcademy.Application.Validators.Media;

public class UploadMediaValidator : AbstractValidator<UploadMediaCommand>
{
    public UploadMediaValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.UploadedById).NotEmpty();
    }
}
