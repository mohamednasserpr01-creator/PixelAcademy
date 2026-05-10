using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Users;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string ParentPhoneNumber { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public Guid? EducationalStageId { get; set; }
    public string? EducationalStageName { get; set; }
    public Guid? EducationStreamId { get; set; }
    public string? EducationStreamName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
