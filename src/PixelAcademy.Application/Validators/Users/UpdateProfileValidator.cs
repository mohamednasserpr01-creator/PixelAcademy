using FluentValidation;
using PixelAcademy.Application.Commands.Users;

namespace PixelAcademy.Application.Validators.Users;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FirstName));
        RuleFor(x => x.LastName).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.LastName));
        RuleFor(x => x.Bio).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Bio));
    }
}
