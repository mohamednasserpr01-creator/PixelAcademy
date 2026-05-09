using FluentValidation;
using PixelAcademy.Application.Commands.Auth;

namespace PixelAcademy.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
