using FluentValidation;
using PixelAcademy.Application.Commands.ActivationCodes;

namespace PixelAcademy.Application.Validators.ActivationCodes;

public class RedeemActivationCodeValidator : AbstractValidator<RedeemActivationCodeCommand>
{
    public RedeemActivationCodeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
