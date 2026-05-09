using FluentValidation;
using PixelAcademy.Application.Commands.ActivationCodes;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Validators.ActivationCodes;

public class GenerateActivationCodeValidator : AbstractValidator<GenerateActivationCodeCommand>
{
    public GenerateActivationCodeValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.GeneratedById).NotEmpty();
        RuleFor(x => x.Value).GreaterThan(0).When(x => x.Type == CodeType.WalletCredit && x.Value.HasValue);
        RuleFor(x => x.CourseId).NotEmpty().When(x => x.Type == CodeType.CourseEnrollment);
        RuleFor(x => x.LectureId).NotEmpty().When(x => x.Type == CodeType.LectureAccess);
        RuleFor(x => x.MaxRedemptions).GreaterThan(0).When(x => x.MaxRedemptions.HasValue);
        RuleFor(x => x.ExpiresAt).GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue);
    }
}
