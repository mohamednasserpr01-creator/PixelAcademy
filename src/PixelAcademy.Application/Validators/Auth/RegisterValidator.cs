using System.Text.RegularExpressions;
using FluentValidation;
using PixelAcademy.Application.Commands.Auth;

namespace PixelAcademy.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    private static readonly Regex EgyptianPhoneRegex = new("^01[0125][0-9]{8}$", RegexOptions.Compiled);

    public RegisterValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(EgyptianPhoneRegex)
            .WithMessage("Phone number must be a valid Egyptian mobile number.");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(10)
            .WithMessage("Full name must be at least 10 characters.");

        RuleFor(x => x.ParentPhoneNumber)
            .NotEmpty()
            .Matches(EgyptianPhoneRegex)
            .WithMessage("Parent phone number must be a valid Egyptian mobile number.");

        RuleFor(x => x.Governorate).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.SchoolName).NotEmpty();
    }
}