using System.Text.RegularExpressions;
using FluentValidation;
using PixelAcademy.Application.Commands.Auth;

namespace PixelAcademy.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    private static readonly Regex EgyptianPhoneRegex = new("^01[0125][0-9]{8}$", RegexOptions.Compiled);

    public LoginValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(EgyptianPhoneRegex)
            .WithMessage("Phone number must be a valid Egyptian mobile number.");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
