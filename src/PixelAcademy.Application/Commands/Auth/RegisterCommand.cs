using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Commands.Auth;

public class RegisterCommand : ICommand<AuthResponseDto>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ParentPhoneNumber { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public Guid? EducationalStageId { get; set; }
    public Guid? EducationStreamId { get; set; }
}